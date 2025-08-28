namespace Camply.Shared.Dtos.Forum
{
    public record ForumSearchRequest(
        string? Title = null,
        List<Guid>? TagIds = null,
        int Skip = 0,
        int Take = 20
    );
}