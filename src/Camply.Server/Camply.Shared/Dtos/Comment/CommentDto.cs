namespace Camply.Shared.Dtos.Comment
{
    public class CommentDto
    {
        public CommentDto(Guid id, string content, Guid authorId, string authorName, Guid? parentId)
        {
            Id = id;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            AuthorId = authorId;
            AuthorUserName = authorName;
            ParentId = parentId;
        }
        
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public string AuthorUserName { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public bool IsEdited { get; set; }
        public string TimeExisted { get; set; } = null!;
        public List<CommentDto>? Replies { get; set; }
    }
}