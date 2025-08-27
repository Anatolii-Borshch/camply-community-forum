namespace Camply.Domain.Entities
{
    public class VoteOption : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; } = null!;
        public int Index { get; set; }
        
        public Guid VoteId { get; set; }
        public virtual Vote Vote { get; set; } = null!;
        
        public virtual ICollection<UserVote> UserVotes { get; set; } = new List<UserVote>();
    }
}