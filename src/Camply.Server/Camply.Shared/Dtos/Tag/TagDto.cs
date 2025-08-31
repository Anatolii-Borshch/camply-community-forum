namespace Camply.Shared.Dtos.Tag
{
    public class TagDto
    {
        public TagDto(Guid id, string name)
        {
            Id = id;            
        }
        
        public Guid Id { get; set; } 
        
        public string Name { get; set; }
    }
}