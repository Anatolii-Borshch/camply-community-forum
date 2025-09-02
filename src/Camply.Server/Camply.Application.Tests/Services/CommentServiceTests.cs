using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Comment;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _commentRepoMock;
        private readonly Mock<IValidator<CommentCreateRequest>> _createValidatorMock;
        private readonly Mock<IValidator<CommentUpdateRequest>> _updateValidatorMock;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _commentRepoMock = new Mock<ICommentRepository>();
            _createValidatorMock = new Mock<IValidator<CommentCreateRequest>>();
            _updateValidatorMock = new Mock<IValidator<CommentUpdateRequest>>();

            _service = new CommentService(
                _commentRepoMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );
        }

        [Fact]
        public async Task GetPostCommentsAsync_Should_Return_Comments()
        {
            var postId = Guid.NewGuid();
            var comments = new List<Comment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    UserId = Guid.NewGuid(),
                    PostId = postId,
                    User = new User
                    {
                        Username = "Test"
                    },
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                }
            };
            _commentRepoMock.Setup(r => r.GetCommentsByPostIdAsync(postId, 10))
                .ReturnsAsync(comments);

            var result = await _service.GetPostCommentsAsync(postId, 10);

            var commentDtos = result.ToList();
            commentDtos.ShouldNotBeEmpty();
            commentDtos.ShouldContain(x => x.Content == "Test");
        }

        [Fact]
        public async Task CreateComment_Should_Add_Comment_When_Valid()
        {
            var request = new CommentCreateRequest(Guid.NewGuid(), Guid.NewGuid(), "Hello", null);
            
            _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            await _service.CreateComment(request);

            _commentRepoMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task CreateComment_Should_Throw_When_Invalid()
        {
            var request = new CommentCreateRequest(new Guid(), Guid.NewGuid(), new string('@', 3000), null);
            _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Content", "Required") }));

            await Should.ThrowAsync<ValidationException>(() => _service.CreateComment(request));
        }

        [Fact]
        public async Task UpdateComment_Should_Update_When_Authorized()
        {
            var userId = Guid.NewGuid();
            var request = new CommentUpdateRequest(Guid.NewGuid(), userId, "Updated");

            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var comment = new Comment { Id = request.Id, UserId = userId, Content = "Old" };
            _commentRepoMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(comment);

            await _service.UpdateComment(request);

            _commentRepoMock.Verify(r => r.UpdateAsync(It.Is<Comment>(c => c.Content == "Updated")), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_Should_Throw_When_NotFound()
        {
            var request = new CommentUpdateRequest(Guid.NewGuid(), Guid.NewGuid(), "test");
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            _commentRepoMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Comment?)null);

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.UpdateComment(request));
        }

        [Fact]
        public async Task UpdateComment_Should_Throw_When_NotAuthor()
        {
            var request = new CommentUpdateRequest(Guid.NewGuid(), Guid.NewGuid(), "test");
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var comment = new Comment { Id = request.Id, UserId = Guid.NewGuid() };
            _commentRepoMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(comment);

            await Should.ThrowAsync<UnauthorizedAccessException>(() => _service.UpdateComment(request));
        }

        [Fact]
        public async Task DeleteComment_Should_Delete_When_Authorized()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var comment = new Comment { Id = id, UserId = userId };

            _commentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(comment);

            await _service.DeleteComment(id, userId);

            _commentRepoMock.Verify(r => r.DeleteAsync(comment), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_Should_Throw_When_NotFound()
        {
            var id = Guid.NewGuid();
            _commentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Comment?)null);

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.DeleteComment(id, Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteComment_Should_Throw_When_NotAuthor()
        {
            var id = Guid.NewGuid();
            var comment = new Comment { Id = id, UserId = Guid.NewGuid() };

            _commentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(comment);

            await Should.ThrowAsync<UnauthorizedAccessException>(() => _service.DeleteComment(id, Guid.NewGuid()));
        }
    }
}