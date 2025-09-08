namespace Camply.Shared.Dtos.Vote
{
    public record VoteCreateRequest(string Title, Guid ForumId, Guid AuthorId, List<VoteOptionCreateDto> VoteOptions);
}