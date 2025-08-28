namespace Camply.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get;  set; } = null!;
        
        public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
    }
}