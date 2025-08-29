using Camply.Domain.Entities;
using Camply.Shared.Dtos.Vote;

namespace Camply.Application.Mappers
{
    public static class VoteMapper
    {
        public static VoteDto MapToVoteDto(this Vote vote, Guid? currentUserId = null)
        {
            var totalVotes = vote.VoteOptions.Sum(x => x.UserVotes.Count);

            return new VoteDto
            {
                Id = vote.Id,
                ForumId = vote.Forum.Id,
                AuthorId = vote.User.Id,
                AuthorUsername = vote.User.Username,
                Title = vote.Title,
                IsEdited = vote.CreatedDate != vote.ModifiedDate,
                VoteOptions = vote.VoteOptions
                    .OrderBy(x => x.Index)
                    .Select(x => x.MapToVoteOptionDto(vote, totalVotes, currentUserId))
                    .ToList()
            };
        }

        public static VoteOptionDto MapToVoteOptionDto(this VoteOption voteOption, Vote vote, int totalVotes, Guid? currentUserId = null)
        {
            int optionVotes = voteOption.UserVotes.Count;

            return new VoteOptionDto
            {
                Id = voteOption.Id,
                Name = voteOption.Name,
                Index = voteOption.Index,
                IsEdited = voteOption.CreatedDate != voteOption.ModifiedDate,
                VotePercentage = totalVotes > 0 ? (float)optionVotes / totalVotes * 100f : 0f,
                IsVoted = currentUserId.HasValue && voteOption.UserVotes.Any(x => x.UserId == currentUserId.Value)
            };
        }
    }
}