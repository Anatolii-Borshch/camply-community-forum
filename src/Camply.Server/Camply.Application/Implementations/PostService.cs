using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ISavedRepository _savedRepository;
        private readonly ILikedPostRepository _likedPostRepository;
        private readonly ISpecifiedRepository<Post> _specification;
        private readonly IValidator<PostCreateRequest> _postCreateValidator;
        private readonly IValidator<PostUpdateRequest> _postUpdateValidator;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository postRepository, ISpecifiedRepository<Post> specification
            , IValidator<PostCreateRequest> postCreateValidator, IValidator<PostUpdateRequest> postUpdateValidator
            , ISavedRepository savedRepository, ILikedPostRepository likedPostRepository, ICommentRepository commentRepository
            , ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _specification = specification;
            _postCreateValidator = postCreateValidator;
            _postUpdateValidator = postUpdateValidator;
            _savedRepository = savedRepository;
            _likedPostRepository = likedPostRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<PostDto>> GetForumPostsAsync(ForumPostsSearchRequest request)
        {
            _logger.LogInformation("Fetching posts for forum {ForumId} with search {@Request}", request.ForumId, request);
            var spec = new PostSearchSpecification(request);

            var posts = await _specification.ListAsync(spec);
            var mappedPosts = posts.Select(x => x.MapToPostDto());
            _logger.LogInformation("Fetched {Count} posts for forum {ForumId}", mappedPosts.Count(), request.ForumId);
            
            return mappedPosts;
        }
        
        public async Task CreatePost(PostCreateRequest request)
        {
            _logger.LogInformation("Creating post '{Title}' by user {UserId} in forum {ForumId}", request.Title, request.AuthorId, request.ForumId);
            var validationResult = _postCreateValidator.Validate(request);
            
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed when creating post: {Errors}", validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }
            
            var post = new Post
            {
                Title = request.Title,
                Content = request.Description,
                UserId = request.AuthorId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ForumId = request.ForumId,
                IsPinned = false
            };

            await _postRepository.AddAsync(post);
            _logger.LogInformation("Post {PostId} created successfully", post.Id);
        }

        public async Task UpdatePost(PostUpdateRequest request)
        {
            _logger.LogInformation("Updating post {PostId} by user {UserId}", request.PostId, request.AuthorId);
            
            var validationResult = _postUpdateValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed when updating post {PostId}: {Errors}", request.PostId, validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }
            
            var post = await _postRepository.GetByIdAsync(request.PostId);

            if (post == null || post.UserId != request.AuthorId)
            {
                _logger.LogWarning("Unauthorized update attempt by user {UserId} on post {PostId}", request.AuthorId, request.PostId);
                throw new UnauthorizedAccessException("You cannot edit this post.");
            }

            post.Title = request.Title;
            post.Content = request.Description;
            post.ModifiedDate = DateTime.UtcNow;

            await _postRepository.UpdateAsync(post);
            
            _logger.LogInformation("Post {PostId} updated successfully", post.Id);
        }

        public async Task DeletePost(Guid userId, Guid postId)
        {
            _logger.LogInformation("Deleting post {PostId} by user {UserId}", postId, userId);
            
            var post = await _postRepository.GetIncludedByIdAsync(postId,
                x => x.LikedPosts,
                x => x.SavedPosts);

            if (post == null || post.UserId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt by user {UserId} on post {PostId}", userId, postId);
                throw new UnauthorizedAccessException("You cannot delete this post.");
            }

            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId, int.MaxValue);

            foreach (var comment in comments)
            {
                await DeleteCommentRecursive(comment);
            }
            
            await _postRepository.DeleteAsync(post);
            
            _logger.LogInformation("Post {PostId} deleted successfully", postId);
        }

        private async Task DeleteCommentRecursive(Comment comment)
        {
            var commentWithReplies = await _commentRepository.GetByIdWithRepliesAsync(comment.Id);

            if (commentWithReplies?.Replies != null)
            {
                foreach (var reply in commentWithReplies.Replies)
                {
                    await DeleteCommentRecursive(reply);
                }
            }

            await _commentRepository.DeleteAsync(commentWithReplies!);
            
            _logger.LogInformation("Deleted comment {CommentId}", commentWithReplies!.Id);
        }

        public async Task<bool> PinPost(Guid userId, Guid postId)
        {
            _logger.LogInformation("Toggling pin for post {PostId} by user {UserId}", postId, userId);
            
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId)
            {
                _logger.LogWarning("Unauthorized pin attempt by user {UserId} on post {PostId}", userId, postId);
                throw new UnauthorizedAccessException("You cannot pin this post.");
            }

            post.IsPinned = !post.IsPinned;
            await _postRepository.UpdateAsync(post);
            
            _logger.LogInformation("Post {PostId} pin status set to {Pinned}", postId, post.IsPinned);

            return post.IsPinned;
        }

        public async Task<bool> LikePost(Guid userId, Guid postId)
        {
            _logger.LogInformation("User {UserId} toggling like for post {PostId}", userId, postId);
            
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post {PostId} not found for like operation", postId);
                throw new KeyNotFoundException("Post not found.");
            }

            var existing = await _likedPostRepository.GetByUserAndPostAsync(userId, postId);
            if (existing != null)
            {
                await _likedPostRepository.DeleteAsync(existing);
                _logger.LogInformation("User {UserId} unliked post {PostId}", userId, postId);
                return false;
            }

            var likedPost = new LikedPost
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await _likedPostRepository.AddAsync(likedPost);

            _logger.LogInformation("User {UserId} liked post {PostId}", userId, postId);
            
            return true;
        }

        public async Task<bool> SavePost(Guid userId, Guid postId)
        {
            _logger.LogInformation("User {UserId} toggling save for post {PostId}", userId, postId);
            
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post {PostId} not found for save operation", postId);
                throw new KeyNotFoundException("Post not found.");
            }

            var existing = await _savedRepository.GetByUserAndPostAsync(userId, postId);
            if (existing != null)
            {
                await _savedRepository.DeleteAsync(existing);
                _logger.LogInformation("User {UserId} unsaved post {PostId}", userId, postId);
                return false;
            }

            var savedPost = new SavedPost
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await _savedRepository.AddAsync(savedPost);

            _logger.LogInformation("User {UserId} saved post {PostId}", userId, postId);
            
            return true;
        }
    }
}