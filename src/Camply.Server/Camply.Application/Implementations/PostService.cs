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
        private readonly ISavedRepository _savedRepository;
        private readonly ILikedPostRepository _likedPostRepository;
        private readonly ISpecifiedRepository<Post> _specification;
        private readonly IValidator<PostCreateRequest> _postCreateValidator;
        private readonly IValidator<PostUpdateRequest> _postUpdateValidator;
        private readonly ICommentRepository _commentRepository;

        public PostService(IPostRepository postRepository, ISpecifiedRepository<Post> specification
            , IValidator<PostCreateRequest> postCreateValidator, IValidator<PostUpdateRequest> postUpdateValidator
            , ISavedRepository savedRepository, ILikedPostRepository likedPostRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _specification = specification;
            _postCreateValidator = postCreateValidator;
            _postUpdateValidator = postUpdateValidator;
            _savedRepository = savedRepository;
            _likedPostRepository = likedPostRepository;
            _commentRepository = commentRepository;
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
                ForumId = request.ForumId,
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
            var post = await _postRepository.GetIncludedByIdAsync(postId,
                x => x.LikedPosts,
                x => x.SavedPosts);

            if (post == null || post.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this post.");

            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId, int.MaxValue);

            foreach (var comment in comments)
            {
                await DeleteCommentRecursive(comment);
            }
            
            await _postRepository.DeleteAsync(post);
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
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            var existing = await _likedPostRepository.GetByUserAndPostAsync(userId, postId);
            if (existing != null)
            {
                await _likedPostRepository.DeleteAsync(existing);
                return false;
            }

            var likedPost = new LikedPost
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await _likedPostRepository.AddAsync(likedPost);

            return true;
        }

        public async Task<bool> SavePost(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            var existing = await _savedRepository.GetByUserAndPostAsync(userId, postId);
            if (existing != null)
            {
                await _savedRepository.DeleteAsync(existing);
                return false;
            }

            var savedPost = new SavedPost
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await _savedRepository.AddAsync(savedPost);

            return true;
        }
    }
}