using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Vote;
using FluentValidation;

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

        public VoteService(IVoteRepository voteRepository, ISpecifiedRepository<Vote> specification
        , IVoteOptionRepository voteOptionRepository, IValidator<VoteCreateRequest> voteCreateValidator
        , IValidator<VoteUpdateRequest> voteUpdateValidator, IValidator<VoteOptionUpdateRequest> voteOptionUpdateValidator
        , IUserVoteRepository userVoteRepository)
        {
            _voteRepository = voteRepository;
            _specification = specification;
            _voteOptionRepository = voteOptionRepository;
            _voteCreateValidator = voteCreateValidator;
            _voteUpdateValidator = voteUpdateValidator;
            _voteOptionUpdateValidator = voteOptionUpdateValidator;
            _userVoteRepository = userVoteRepository;
        }
        
        public async Task<IEnumerable<VoteDto>> GetForumVotesAsync(ForumVotesSearchRequest request)
        {
            var spec = new VoteSearchSpecification(request);
            var votes = await _specification.ListAsync(spec);
            var mappedVotes = votes.Select(x => x.MapToVoteDto());
            
            return mappedVotes;
        }

       public async Task CreateVote(VoteCreateRequest request)
        {
            var validationResult = _voteCreateValidator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
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
        }

        public async Task UpdateVote(VoteUpdateRequest request)
        {
            var validationResult = _voteUpdateValidator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var vote = await _voteRepository.GetByIdAsync(request.VoteId);
            
            if (vote == null) 
                throw new KeyNotFoundException("Vote not found");
            
            if (vote.UserId != request.UserId) 
                throw new UnauthorizedAccessException("Not allowed to update this vote");

            vote.Title = request.Title;
            vote.ModifiedDate = DateTime.UtcNow;

            await _voteRepository.UpdateAsync(vote);
        }

        public async Task UpdateVoteOption(VoteOptionUpdateRequest request)
        {
            var validationResult = _voteOptionUpdateValidator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var option = await _voteOptionRepository.GetByIdAsync(request.VoteOptionId);
            if (option == null)
                throw new KeyNotFoundException("Vote option not found");

            if (option.Vote.UserId != request.AuthorId)
                throw new UnauthorizedAccessException("Not allowed to update this vote option");

            option.Name = request.Name;
            option.Index = request.Index;
            option.ModifiedDate = DateTime.UtcNow;

            await _voteOptionRepository.UpdateAsync(option);

            await RecalculateVoteOptionIndexes(option.VoteId);
        }

        public async Task DeleteVoteOption(Guid optionId, Guid userId)
        {
            var option = await _voteOptionRepository.GetByIdAsync(optionId);
            if (option == null)
                throw new KeyNotFoundException("Vote option not found");

            if (option.Vote.UserId != userId)
                throw new UnauthorizedAccessException("Not allowed to delete this vote option");

            foreach (var item in option.UserVotes.ToList())
                await _userVoteRepository.DeleteAsync(item);

            await _voteOptionRepository.DeleteAsync(option);

            await RecalculateVoteOptionIndexes(option.VoteId);
        }


        public async Task DeleteVote(Guid voteId, Guid userId)
        {
            var vote = await _voteRepository.GetByIdAsync(voteId);
            
            if (vote == null) 
                throw new KeyNotFoundException("Vote not found");
            
            if (vote.UserId != userId) 
                throw new UnauthorizedAccessException("Not allowed");

            await _voteRepository.DeleteAsync(vote);
        }

        
        public async Task<bool> Vote(Guid optionId, Guid userId)
        {
            var option = await _voteOptionRepository.GetByIdAsync(optionId);
            
            if (option == null)
                throw new KeyNotFoundException("Vote option not found");

            var voteId = option.VoteId;

            var existingVote = await _userVoteRepository.GetByVoteAndUserAsync(voteId, userId);

            if (existingVote != null)
            {
                if (existingVote.OptionId == optionId)
                {
                    await _userVoteRepository.DeleteAsync(existingVote);
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
        }
    }
}