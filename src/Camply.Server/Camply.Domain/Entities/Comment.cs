namespace Camply.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Content { get; set; } = null!;
        
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
        
        public Guid? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }
        
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}