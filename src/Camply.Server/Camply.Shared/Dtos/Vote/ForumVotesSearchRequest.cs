namespace Camply.Shared.Dtos.Vote
{
    public record ForumVotesSearchRequest(
        Guid CurrentUserId,
        Guid? ForumId = null,
        Guid? AuthorId = null,
        Guid? VoteId = null,
        string? Title = null,
        int Skip = 0,
        int Take = 20,
        string? OrderBy = null,
        bool OrderDescending = false
        );
}