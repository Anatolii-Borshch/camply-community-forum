using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;

namespace Camply.Application.Contracts.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetForumPostsAsync(ForumPostsSearchRequest request);
        Task CreatePost(PostCreateRequest request);
        Task UpdatePost(PostUpdateRequest request);
        Task DeletePost(Guid userId, Guid postId);
        Task<bool> PinPost(Guid userId, Guid postId);
        Task<bool> LikePost(Guid userId, Guid postId);
        Task<bool> SavePost(Guid userId, Guid postId);
    }
}