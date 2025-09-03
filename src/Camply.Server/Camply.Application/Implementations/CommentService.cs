using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Comment;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class CommentService : UserPermissionService, ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        
        private readonly IValidator<CommentCreateRequest> _createValidator;
        private readonly IValidator<CommentUpdateRequest> _updateValidator;
        
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, IValidator<CommentCreateRequest> createValidator,
            IValidator<CommentUpdateRequest> updateValidator, IUserRepository userRepository,ILogger<CommentService> logger) : base(userRepository, logger)
        {
            _commentRepository = commentRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<IEnumerable<CommentDto>> GetPostCommentsAsync(Guid postId, int numberOfComments)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId, numberOfComments);

            var mappedComments = comments
                .Select(x => x.MapToCommentDto())
                .ToList();

            return mappedComments;
        }

        public async Task CreateComment(CommentCreateRequest request)
        {
            _logger.LogInformation("Creating comment for post {PostId} by user {UserId}", request.PostId, request.UserId);
            
            await EnsureUserExistsAsync(request.UserId);
            
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while creating comment: {Errors}", validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var comment = new Comment
            {
                Content = request.Content,
                UserId = request.UserId,
                PostId = request.PostId,
                ParentCommentId = request.ParentCommentId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            
            _logger.LogInformation("Comment created with id {CommentId}", comment.Id);
        }

        public async Task UpdateComment(CommentUpdateRequest request)
        {
            _logger.LogInformation("Updating comment {CommentId} by user {UserId}", request.Id, request.UserId);
            
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while updating comment {CommentId}: {Errors}", request.Id, validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for update", request.Id);
                throw new KeyNotFoundException($"Comment with id {request.Id} not found.");
            }

            var user = await EnsureUserExistsAsync(request.UserId);
            EnsureUserAcess(user, comment.UserId);

            comment.Content = request.Content;
            comment.ModifiedDate = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);

            _logger.LogInformation("Comment {CommentId} updated successfully", comment.Id);
        }

        public async Task DeleteComment(Guid id, Guid userId)
        {
            _logger.LogInformation("Deleting comment {CommentId} by user {UserId}", id, userId);

            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found for deletion", id);
                throw new KeyNotFoundException($"Comment with id {id} not found.");
            }

            var user = await EnsureUserExistsAsync(userId);
            EnsureUserAcess(user, comment.UserId);

            await _commentRepository.DeleteAsync(comment);

            _logger.LogInformation("Comment {CommentId} deleted successfully", id);
        }
    }
}