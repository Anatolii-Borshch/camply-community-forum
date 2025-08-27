namespace Camply.Domain.Entities
{
    public class UserVote : BaseEntity
    {
        public DateTime VotedDate { get; set; }
        
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public Guid OptionId { get; set; }
        public virtual VoteOption Option { get; set; } = null!;
    }
}