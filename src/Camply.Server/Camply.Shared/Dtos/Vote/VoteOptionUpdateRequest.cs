namespace Camply.Shared.Dtos.Vote
{
    public record VoteOptionUpdateRequest(Guid VoteOptionId, Guid AuthorId, string Name, int Index);
}