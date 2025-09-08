namespace Camply.Domain.Entities
{
    public class Forum : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        
        public Guid? AdminId { get; set; }
        public virtual User? Admin { get; set; }
        
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}