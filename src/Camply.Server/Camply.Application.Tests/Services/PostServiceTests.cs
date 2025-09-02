using System.Linq.Expressions;
using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _postRepoMock = new();
        private readonly Mock<ISavedRepository> _savedRepoMock = new();
        private readonly Mock<ILikedPostRepository> _likedRepoMock = new();
        private readonly Mock<ISpecifiedRepository<Post>> _specificationMock = new();
        private readonly Mock<IValidator<PostCreateRequest>> _createValidatorMock = new();
        private readonly Mock<IValidator<PostUpdateRequest>> _updateValidatorMock = new();
        private readonly Mock<ICommentRepository> _commentRepoMock = new();

        private readonly PostService _service;

        public PostServiceTests()
        {
            _service = new PostService(
                _postRepoMock.Object,
                _specificationMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _savedRepoMock.Object,
                _likedRepoMock.Object,
                _commentRepoMock.Object
            );
        }

        [Fact]
        public async Task GetForumPostsAsync_Should_Return_Mapped_Posts()
        {
            var request = new ForumPostsSearchRequest();
            var posts = new List<Post> { new Post { Id = Guid.NewGuid(), Title = "Test" } };

            _specificationMock.Setup(x => x.ListAsync(It.IsAny<PostSearchSpecification>()))
                .ReturnsAsync(posts);

            var result = await _service.GetForumPostsAsync(request);

            var postDtos = result.ToList();
            postDtos.ShouldNotBeEmpty();
            postDtos.ShouldContain(x => x.Title == "Test");
        }

        [Fact]
        public async Task CreatePost_Should_Add_Post_When_Valid()
        {
            var request = new PostCreateRequest("Title", "Desc", Guid.NewGuid(), Guid.NewGuid());
            _createValidatorMock.Setup(x => x.Validate(request))
                .Returns(new ValidationResult());

            await _service.CreatePost(request);

            _postRepoMock.Verify(r => r.AddAsync(It.Is<Post>(p => p.Title == "Title")), Times.Once);
        }

        [Fact]
        public async Task CreatePost_Should_Throw_When_Invalid()
        {
            var request = new PostCreateRequest("", "", Guid.NewGuid(), Guid.NewGuid());
            _createValidatorMock.Setup(x => x.Validate(request))
                .Returns(new ValidationResult(new[] { new ValidationFailure("Title", "Required") }));

            await Should.ThrowAsync<ValidationException>(() => _service.CreatePost(request));
        }

        [Fact]
        public async Task UpdatePost_Should_Update_When_Valid_And_Authorized()
        {
            var userId = Guid.NewGuid();
            var request = new PostUpdateRequest(Guid.NewGuid(), "Updated", "Desc", userId);
            var post = new Post { Id = request.PostId, UserId = userId, Title = "Old" };

            _updateValidatorMock.Setup(x => x.Validate(request))
                .Returns(new ValidationResult());
            _postRepoMock.Setup(r => r.GetByIdAsync(request.PostId)).ReturnsAsync(post);

            await _service.UpdatePost(request);

            _postRepoMock.Verify(r => r.UpdateAsync(It.Is<Post>(p => p.Title == "Updated")), Times.Once);
        }

        [Fact]
        public async Task UpdatePost_Should_Throw_When_Unauthorized()
        {
            var request = new PostUpdateRequest(Guid.NewGuid(), "Updated", "Desc", Guid.NewGuid());
            var post = new Post { Id = request.PostId, UserId = Guid.NewGuid() };

            _updateValidatorMock.Setup(x => x.Validate(request)).Returns(new ValidationResult());
            _postRepoMock.Setup(r => r.GetByIdAsync(request.PostId)).ReturnsAsync(post);

            await Should.ThrowAsync<UnauthorizedAccessException>(() => _service.UpdatePost(request));
        }

        [Fact]
        public async Task DeletePost_Should_Delete_Post_And_Comments()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, UserId = userId };

            var comment = new Comment { Id = Guid.NewGuid() };

            _postRepoMock
                .Setup(r => r.GetIncludedByIdAsync(postId, It.IsAny<Expression<Func<Post, object>>>(), It.IsAny<Expression<Func<Post, object>>>()))
                .ReturnsAsync(post);

            _commentRepoMock
                .Setup(r => r.GetCommentsByPostIdAsync(postId, int.MaxValue))
                .ReturnsAsync(new List<Comment> { comment });

            _commentRepoMock
                .Setup(r => r.GetByIdWithRepliesAsync(comment.Id))
                .ReturnsAsync(comment);

            await _service.DeletePost(userId, postId);

            _commentRepoMock.Verify(r => r.DeleteAsync(comment), Times.Once);
            _postRepoMock.Verify(r => r.DeleteAsync(post), Times.Once);
        }

        [Fact]
        public async Task DeletePost_Should_Throw_When_Unauthorized()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, UserId = Guid.NewGuid() };

            _postRepoMock
                .Setup(r => r.GetIncludedByIdAsync(postId, It.IsAny<Expression<Func<Post, object>>>(), It.IsAny<Expression<Func<Post, object>>>()))
                .ReturnsAsync(post);

            await Should.ThrowAsync<UnauthorizedAccessException>(() => _service.DeletePost(userId, postId));
        }

        [Fact]
        public async Task PinPost_Should_Toggle_Pin()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, UserId = userId, IsPinned = false };

            _postRepoMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

            var result = await _service.PinPost(userId, postId);

            result.ShouldBeTrue();
            _postRepoMock.Verify(r => r.UpdateAsync(It.Is<Post>(p => p.IsPinned)), Times.Once);
        }

        [Fact]
        public async Task LikePost_Should_Add_If_Not_Exists()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postRepoMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _likedRepoMock.Setup(r => r.GetByUserAndPostAsync(userId, postId)).ReturnsAsync((LikedPost?)null);

            var result = await _service.LikePost(userId, postId);

            result.ShouldBeTrue();
            _likedRepoMock.Verify(r => r.AddAsync(It.Is<LikedPost>(lp => lp.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task LikePost_Should_Remove_If_Exists()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };
            var liked = new LikedPost { PostId = postId, UserId = userId };

            _postRepoMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _likedRepoMock.Setup(r => r.GetByUserAndPostAsync(userId, postId)).ReturnsAsync(liked);

            var result = await _service.LikePost(userId, postId);

            result.ShouldBeFalse();
            _likedRepoMock.Verify(r => r.DeleteAsync(liked), Times.Once);
        }

        [Fact]
        public async Task SavePost_Should_Add_If_Not_Exists()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postRepoMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _savedRepoMock.Setup(r => r.GetByUserAndPostAsync(userId, postId)).ReturnsAsync((SavedPost?)null);

            var result = await _service.SavePost(userId, postId);

            result.ShouldBeTrue();
            _savedRepoMock.Verify(r => r.AddAsync(It.Is<SavedPost>(sp => sp.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task SavePost_Should_Remove_If_Exists()
        {
            var userId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };
            var saved = new SavedPost { PostId = postId, UserId = userId };

            _postRepoMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);
            _savedRepoMock.Setup(r => r.GetByUserAndPostAsync(userId, postId)).ReturnsAsync(saved);

            var result = await _service.SavePost(userId, postId);

            result.ShouldBeFalse();
            _savedRepoMock.Verify(r => r.DeleteAsync(saved), Times.Once);
        }
    }
}