using Camply.Domain.Entities;
using Camply.Shared.Dtos.Tag;

namespace Camply.Shared.Dtos.Forum
{
    public class ForumDto
    {
        public ForumDto(Guid id, string title, string? description
            , int postCount, string timeExisted, List<TagDto> tags)
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description;
            PostCount = postCount;
            TimeExisted = timeExisted;
            Tags = tags;
        }
        
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int PostCount { get; set; }
        public string TimeExisted { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}