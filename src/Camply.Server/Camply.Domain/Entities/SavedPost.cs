namespace Camply.Domain.Entities
{
    public class SavedPost : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        
        public Guid UserId { get; set; }
        public virtual User User { get; set; } =  null!;
        
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } =  null!;
    }
}