namespace Camply.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; } = null!;
        
        public Guid ForumId { get; set; }
        public virtual Forum Forum { get; set; } = null!;
        
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        
        public virtual ICollection<VoteOption> VoteOptions { get; set; } = new List<VoteOption>();
    }
}