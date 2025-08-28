namespace Camply.Shared.Dtos.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public bool IsEdited { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public bool IsPinned { get; set; }
        public string AuthorUsername { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public int Likes { get; set; }
        public int CommentsNumber { get; set; }
        public string ExistedTime { get; set; } = null!;
    }
}