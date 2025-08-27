namespace Camply.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; private set; } = null!;
        
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}