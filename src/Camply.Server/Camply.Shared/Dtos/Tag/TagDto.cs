namespace Camply.Shared.Dtos.Tag
{
    public class TagDto
    {
        public TagDto(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            
            Name = name;
        }
        public string Name { get; set; } = null!;
    }
}