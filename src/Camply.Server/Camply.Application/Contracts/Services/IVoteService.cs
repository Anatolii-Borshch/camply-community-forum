using Camply.Shared.Dtos.Vote;

namespace Camply.Application.Contracts.Services
{
    public interface IVoteService
    {
        Task<IEnumerable<VoteDto>> GetForumVotesAsync(ForumVotesSearchRequest request);
        Task CreateVote(VoteCreateRequest request);
        Task UpdateVote(VoteUpdateRequest request);
        Task UpdateVoteOption(VoteOptionUpdateRequest request);
        Task DeleteVoteOption(Guid optionId, Guid userId);
        Task DeleteVote(Guid vote, Guid userId);
        Task<bool> Vote(Guid optionId, Guid userId);
    }
}