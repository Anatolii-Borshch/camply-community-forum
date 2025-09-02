using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Vote;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class VoteService : IVoteService
    {
        private readonly ISpecifiedRepository<Vote> _specification;
        private readonly IVoteRepository _voteRepository;
        private readonly IVoteOptionRepository _voteOptionRepository;
        private readonly IUserVoteRepository _userVoteRepository;
        
        private readonly IValidator<VoteCreateRequest> _voteCreateValidator;
        private readonly IValidator<VoteUpdateRequest> _voteUpdateValidator;
        private readonly IValidator<VoteOptionUpdateRequest> _voteOptionUpdateValidator;
        private readonly ILogger<VoteService> _logger;

        public VoteService(IVoteRepository voteRepository, ISpecifiedRepository<Vote> specification
        , IVoteOptionRepository voteOptionRepository, IValidator<VoteCreateRequest> voteCreateValidator
        , IValidator<VoteUpdateRequest> voteUpdateValidator, IValidator<VoteOptionUpdateRequest> voteOptionUpdateValidator
        , IUserVoteRepository userVoteRepository, ILogger<VoteService> logger)
        {
            _voteRepository = voteRepository;
            _specification = specification;
            _voteOptionRepository = voteOptionRepository;
            _voteCreateValidator = voteCreateValidator;
            _voteUpdateValidator = voteUpdateValidator;
            _voteOptionUpdateValidator = voteOptionUpdateValidator;
            _userVoteRepository = userVoteRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<VoteDto>> GetForumVotesAsync(ForumVotesSearchRequest request)
        {
            _logger.LogInformation("Fetching votes for forum {ForumId} by user {UserId}", request.ForumId, request.CurrentUserId);
            
            var spec = new VoteSearchSpecification(request);
            var votes = await _specification.ListAsync(spec);
            var mappedVotes = votes.Select(x => x.MapToVoteDto(request.CurrentUserId));
            
            _logger.LogInformation("Fetched {Count} votes for forum {ForumId}", mappedVotes.Count(), request.ForumId);
            return mappedVotes;
        }

       public async Task CreateVote(VoteCreateRequest request)
        {
            _logger.LogInformation("Creating vote in forum {ForumId} by user {UserId}", request.ForumId, request.AuthorId);
            var validationResult = _voteCreateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Vote creation validation failed for forum {ForumId}", request.ForumId);
                throw new ValidationException(validationResult.Errors);
            }
            
            var newVote = new Vote
            {
                ForumId = request.ForumId,
                UserId = request.AuthorId,
                Title = request.Title,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                VoteOptions = request.VoteOptions
                    .Select((x, i) => new VoteOption
                    {
                        Name = x.Name,
                        Index = i,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    }).ToList()
            };

            await _voteRepository.AddAsync(newVote);
            
            _logger.LogInformation("Vote created successfully in forum {ForumId} with id {VoteId}", request.ForumId, newVote.Id);
        }

        public async Task UpdateVote(VoteUpdateRequest request)
        {
            _logger.LogInformation("Updating vote {VoteId} by user {UserId}", request.VoteId, request.UserId);
            var validationResult = _voteUpdateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Vote update validation failed for vote {VoteId}", request.VoteId);
                throw new ValidationException(validationResult.Errors);
            }
            
            var vote = await _voteRepository.GetByIdAsync(request.VoteId);
            if (vote == null)
            {
                _logger.LogWarning("Vote {VoteId} not found", request.VoteId);
                throw new KeyNotFoundException("Vote not found");
            }
            
            if (vote.UserId != request.UserId)
            {
                _logger.LogWarning("User {UserId} not allowed to update vote {VoteId}", request.UserId, request.VoteId);
                throw new UnauthorizedAccessException("Not allowed to update this vote");
            }

            vote.Title = request.Title;
            vote.ModifiedDate = DateTime.UtcNow;

            await _voteRepository.UpdateAsync(vote);
            
            _logger.LogInformation("Vote {VoteId} updated successfully", request.VoteId);
        }

        public async Task UpdateVoteOption(VoteOptionUpdateRequest request)
        {
            _logger.LogInformation("Updating vote option {OptionId} by user {UserId}", request.VoteOptionId, request.AuthorId);
            var validationResult = _voteOptionUpdateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Vote option update validation failed for option {OptionId}", request.VoteOptionId);
                throw new ValidationException(validationResult.Errors);
            }

            var option = await _voteOptionRepository.GetByIdAsync(request.VoteOptionId);
            if (option == null)
            {
                _logger.LogWarning("Vote option {OptionId} not found", request.VoteOptionId);
                throw new KeyNotFoundException("Vote option not found");
            }

            if (option.Vote.UserId != request.AuthorId)
            {
                _logger.LogWarning("User {UserId} not allowed to update vote option {OptionId}", request.AuthorId, request.VoteOptionId);
                throw new UnauthorizedAccessException("Not allowed to update this vote option");
            }

            option.Name = request.Name;
            option.Index = request.Index;
            option.ModifiedDate = DateTime.UtcNow;

            await _voteOptionRepository.UpdateAsync(option);

            await RecalculateVoteOptionIndexes(option.VoteId);
            
            _logger.LogInformation("Vote option {OptionId} updated successfully", request.VoteOptionId);
        }

        public async Task DeleteVoteOption(Guid optionId, Guid userId)
        {
            _logger.LogInformation("Deleting vote option {OptionId} by user {UserId}", optionId, userId);
            var option = await _voteOptionRepository.GetByIdAsync(optionId);
            if (option == null)
            {
                _logger.LogWarning("Vote option {OptionId} not found", optionId);
                throw new KeyNotFoundException("Vote option not found");
            }

            if (option.Vote.UserId != userId)
            {
                _logger.LogWarning("User {UserId} not allowed to delete vote option {OptionId}", userId, optionId);
                throw new UnauthorizedAccessException("Not allowed to delete this vote option");
            }

            foreach (var item in option.UserVotes.ToList())
                await _userVoteRepository.DeleteAsync(item);

            await _voteOptionRepository.DeleteAsync(option);
            await RecalculateVoteOptionIndexes(option.VoteId);
            
            _logger.LogInformation("Vote option {OptionId} deleted successfully", optionId);
        }


        public async Task DeleteVote(Guid voteId, Guid userId)
        {
            _logger.LogInformation("Deleting vote {VoteId} by user {UserId}", voteId, userId);
            var vote = await _voteRepository.GetByIdAsync(voteId);
            
            if (vote == null)
            {
                _logger.LogWarning("Vote {VoteId} not found", voteId);
                throw new KeyNotFoundException("Vote not found");
            }

            if (vote.UserId != userId)
            {
                _logger.LogWarning("User {UserId} not allowed to delete vote {VoteId}", userId, voteId);
                throw new UnauthorizedAccessException("Not allowed");
            }

            await _voteRepository.DeleteAsync(vote);
            
            _logger.LogInformation("Vote {VoteId} deleted successfully", voteId);
        }

        
        public async Task<bool> Vote(Guid optionId, Guid userId)
        {
            _logger.LogInformation("User {UserId} voting for option {OptionId}", userId, optionId);
            var option = await _voteOptionRepository.GetByIdAsync(optionId);
            if (option == null)
            {
                _logger.LogWarning("Vote option {OptionId} not found", optionId);
                throw new KeyNotFoundException("Vote option not found");
            }

            var voteId = option.VoteId;
            var existingVote = await _userVoteRepository.GetByVoteAndUserAsync(voteId, userId);

            if (existingVote != null)
            {
                if (existingVote.OptionId == optionId)
                {
                    await _userVoteRepository.DeleteAsync(existingVote);
                    _logger.LogInformation("User {UserId} removed vote for option {OptionId}", userId, optionId);
                    return false;
                }

                await _userVoteRepository.DeleteAsync(existingVote);
            }

            var newVote = new UserVote
            {
                UserId = userId,
                OptionId = optionId,
                VotedDate = DateTime.UtcNow
            };

            await _userVoteRepository.AddAsync(newVote);
            
            _logger.LogInformation("User {UserId} voted successfully for option {OptionId}", userId, optionId);
            return true; 
        }
        
        private async Task RecalculateVoteOptionIndexes(Guid voteId)
        {
            var options = await _voteOptionRepository.GetAllAsync();
            var voteOptions = options
                .Where(o => o.VoteId == voteId)
                .OrderBy(o => o.Index)
                .ToList();

            for (int i = 0; i < voteOptions.Count; i++)
            {
                var opt = voteOptions[i];
                if (opt.Index != i)
                {
                    opt.Index = i;
                    await _voteOptionRepository.UpdateAsync(opt);
                }
            }
            
            _logger.LogInformation("Recalculated indexes for vote {VoteId}", voteId);
        }
    }
}