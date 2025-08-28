namespace Camply.Domain.Entities
{
    public class Post : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public bool IsPinned { get; set; }
        
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        public virtual ICollection<LikedPost> LikedPosts { get; set; } = new List<LikedPost>();
    }
}