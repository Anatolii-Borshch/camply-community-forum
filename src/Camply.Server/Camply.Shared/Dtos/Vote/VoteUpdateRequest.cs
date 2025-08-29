namespace Camply.Shared.Dtos.Vote
{
    public record VoteUpdateRequest(Guid VoteId, Guid UserId, string Title);
}