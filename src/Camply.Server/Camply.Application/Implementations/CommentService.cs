using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Comment;
using FluentValidation;

namespace Camply.Application.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IValidator<CommentCreateRequest> _createValidator;
        private readonly IValidator<CommentUpdateRequest> _updateValidator;

        public CommentService(ICommentRepository commentRepository,
                              IValidator<CommentCreateRequest> createValidator,
                              IValidator<CommentUpdateRequest> updateValidator)
        {
            _commentRepository = commentRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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
        }

        public async Task UpdateComment(CommentUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
                throw new KeyNotFoundException($"Comment with id {request.Id} not found.");

            if (comment.UserId != request.UserId)
                throw new UnauthorizedAccessException("User is not the author of this comment.");

            comment.Content = request.Content;
            comment.ModifiedDate = DateTime.UtcNow;

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task DeleteComment(Guid id, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException($"Comment with id {id} not found.");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("User is not the author of this comment.");

            await _commentRepository.DeleteAsync(comment);
        }
    }
}