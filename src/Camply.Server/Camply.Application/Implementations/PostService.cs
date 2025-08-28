using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;
using FluentValidation;

namespace Camply.Application.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ISpecifiedRepository<Post> _specification;
        private readonly IValidator<PostCreateRequest> _postCreateValidator;
        private readonly IValidator<PostUpdateRequest> _postUpdateValidator;

        public PostService(IPostRepository postRepository, ISpecifiedRepository<Post> specification
            , IValidator<PostCreateRequest> postCreateValidator, IValidator<PostUpdateRequest> postUpdateValidator)
        {
            _postRepository = postRepository;
            _specification = specification;
            _postCreateValidator = postCreateValidator;
            _postUpdateValidator = postUpdateValidator;
        }
        
        public async Task<IEnumerable<PostDto>> GetForumPostsAsync(ForumPostsSearchRequest request)
        {
            var spec = new PostSearchSpecification(request);

            var posts = await _specification.ListAsync(spec);
            var mappedPosts = posts.Select(x => x.MapToPostDto());
            
            return mappedPosts;
        }
        
        public async Task CreatePost(PostCreateRequest request)
        {
            var validationResult = _postCreateValidator.Validate(request);
            
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var post = new Post
            {
                Title = request.Title,
                Content = request.Description,
                UserId = request.AuthorId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IsPinned = false
            };

            await _postRepository.AddAsync(post);
        }

        public async Task UpdatePost(PostUpdateRequest request)
        {
            var validationResult = _postUpdateValidator.Validate(request);
            
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
            
            var post = await _postRepository.GetByIdAsync(request.PostId);

            if (post == null || post.UserId != request.AuthorId)
                throw new UnauthorizedAccessException("You cannot edit this post.");

            post.Title = request.Title;
            post.Content = request.Description;
            post.ModifiedDate = DateTime.UtcNow;

            await _postRepository.UpdateAsync(post);
        }

        public async Task DeletePost(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null || post.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this post.");

            await _postRepository.DeleteAsync(post);
        }

        public async Task<bool> PinPost(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null || post.UserId != userId)
                throw new UnauthorizedAccessException("You cannot pin this post.");

            post.IsPinned = !post.IsPinned;
            await _postRepository.UpdateAsync(post);

            return post.IsPinned;
        }

        public async Task<bool> LikePost(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetIncludedByIdAsync(postId, includes: x => x.LikedPosts);

            if (post == null || post.UserId != userId)
                throw new UnauthorizedAccessException("You cannot pin this post.");

            var existing = post.LikedPosts.FirstOrDefault(x => x.UserId == userId);
            if (existing != null)
            {
                post.LikedPosts.Remove(existing);
                await _postRepository.UpdateAsync(post);
                return false;
            }

            post.LikedPosts.Add(new LikedPost { CreatedDate = DateTime.Now, PostId = postId, UserId = userId });
            await _postRepository.UpdateAsync(post);
            return true;
        }

        public async Task<bool> SavePost(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetIncludedByIdAsync(postId, includes: x => x.SavedPosts);

            if (post == null || post.UserId != userId)
                throw new UnauthorizedAccessException("You cannot pin this post.");

            var existing = post.SavedPosts.FirstOrDefault(x => x.UserId == userId);
            if (existing != null)
            {
                post.SavedPosts.Remove(existing);
                await _postRepository.UpdateAsync(post);
                return false;
            }

            post.SavedPosts.Add(new SavedPost { CreatedDate = DateTime.Now ,PostId = postId, UserId = userId });
            await _postRepository.UpdateAsync(post);
            return true;
        }
    }
}