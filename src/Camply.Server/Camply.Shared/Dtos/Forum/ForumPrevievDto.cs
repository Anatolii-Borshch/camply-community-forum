using Camply.Shared.Dtos.Tag;

namespace Camply.Shared.Dtos.Forum
{
    public class ForumPrevievDto
    {
        public ForumPrevievDto(Guid id, string title, string description, List<TagDto> tags)
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description;
            Tags = tags;
        }
        
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}