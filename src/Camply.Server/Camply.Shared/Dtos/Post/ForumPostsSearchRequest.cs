namespace Camply.Shared.Dtos.Post
{
    public record ForumPostsSearchRequest(
        Guid? ForumId = null,
        Guid? AuthorId = null,
        string? Title = null,
        bool? IsPinned = null,
        DateTime? CreatedAfter = null,
        DateTime? CreatedBefore = null,
        string OrderBy = "createdDate",
        bool Descending = true,
        int Skip = 0,
        int Take = 20
    );
}