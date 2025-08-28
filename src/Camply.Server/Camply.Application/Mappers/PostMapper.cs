using Camply.Application.Helpers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;

namespace Camply.Application.Mappers
{
    public static class PostMapper
    {
        public static PostDto MapToPostDto(this Post post)
        {
            return new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorId = post.UserId,
                AuthorUsername = post.User.Username,
                CommentsNumber = post.Comments.Count,
                ExistedTime = TimeHelper.GetTimeExisted(post.CreatedDate),
                IsEdited = post.CreatedDate == post.ModifiedDate,
                IsPinned = post.IsPinned,
                Likes = post.LikedPosts.Count,
            };
        }
    }
}