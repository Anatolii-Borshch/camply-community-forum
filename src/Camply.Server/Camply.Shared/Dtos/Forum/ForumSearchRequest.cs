namespace Camply.Shared.Dtos.Forum
{
    public record ForumSearchRequest(
        string? Title = null,
        List<Guid>? Tags = null,
        int Skip = 0,
        int Take = 20
    );
}