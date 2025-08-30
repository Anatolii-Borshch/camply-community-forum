using Camply.Application.Helpers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Tag;

namespace Camply.Application.Mappers
{
    public static class ForumMapper
    {
        public static ForumDto MapToForumDto(this Forum forum)
        {
            var postCount = forum.Posts.Count;
            var existenceTime = TimeHelper.GetTimeExisted(forum.CreatedDate);
            
            return new ForumDto(forum.Id, forum.Title, forum.Description
                , postCount, existenceTime
                , forum.Tags.Select(x => new TagDto(x.Name)).ToList());
        }
        
        public static ForumPrevievDto MapToForumPreviewDto(this Forum forum)
        {
            return new ForumPrevievDto(forum.Id, forum.Title, forum.Description, forum.Tags.Select(x => new TagDto(x.Name)).ToList());
        }
    }
}