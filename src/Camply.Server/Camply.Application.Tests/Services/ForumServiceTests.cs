using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Domain.Entities;
using Camply.Domain.Enums;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Tag;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class ForumServiceTests
    {
        private readonly Mock<IForumRepository> _forumRepoMock = new();
        private readonly Mock<IValidator<ForumCreateRequest>> _createValidatorMock = new();
        private readonly Mock<IValidator<ForumUpdateRequest>> _updateValidatorMock = new();
        private readonly Mock<ISpecifiedRepository<Forum>> _specifiedRepoMock = new();
        private readonly Mock<ITagRepository> _tagRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly ForumService _service;

        public ForumServiceTests()
        {
            _service = new ForumService(
                _forumRepoMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _specifiedRepoMock.Object,
                _tagRepoMock.Object,
                NullLogger<ForumService>.Instance,
                _userRepoMock.Object
                );

            _createValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ForumCreateRequest>(), default))
                .ReturnsAsync(new ValidationResult());

            _updateValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ForumUpdateRequest>(), default))
                .ReturnsAsync(new ValidationResult());
        }

        [Fact]
        public async Task GetForumById_Should_Return_ForumDto_When_Found()
        {
            var forumId = Guid.NewGuid();
            var forum = new Forum { Id = forumId, Title = "Test Forum", Description = "Desc" };

            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync(forum);

            var result = await _service.GetForumById(forumId);

            result.ShouldNotBeNull();
            result.Title.ShouldBe("Test Forum");
        }

        [Fact]
        public async Task GetForumById_Should_Throw_When_Not_Found()
        {
            var forumId = Guid.NewGuid();
            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync((Forum?)null);

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.GetForumById(forumId));
        }

        [Fact]
        public async Task CreateForum_Should_Add_Forum_When_Valid()
        {
            var tagId = Guid.NewGuid();
            var tag = new Tag { Id = tagId, Name = "CSharp" };

            _tagRepoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync(tag);

            var request = new ForumCreateRequest("Title", "Desc", Guid.NewGuid(), new List<TagDto> { new(tagId, "CSharp") });

            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User());
            
            await _service.CreateForum(request);

            _forumRepoMock.Verify(r => r.AddAsync(It.Is<Forum>(f =>
                f.Title == "Title" &&
                f.Tags.Contains(tag)
            )), Times.Once);
        }

        [Fact]
        public async Task CreateForum_Should_Throw_When_Tag_Not_Found()
        {
            var tagId = Guid.NewGuid();
            _tagRepoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync((Tag?)null);

            var request = new ForumCreateRequest("Title", "Desc", Guid.NewGuid(), new List<TagDto> { new(tagId, "NotFound") });

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.CreateForum(request));
        }

        [Fact]
        public async Task UpdateForum_Should_Update_When_Valid()
        {
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var tagId = Guid.NewGuid();
            
            var existingForum = new Forum { Id = forumId, Title = "Old", Description = "OldDesc", Tags = new List<Tag>(), AdminId = userId};
            var tag = new Tag { Id = tagId, Name = "UpdatedTag" };
            
            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User() { Id = Guid.NewGuid(), Role = UserRole.Administrator });
            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync(existingForum);
            _tagRepoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync(tag);

            var request = new ForumUpdateRequest(forumId, "NewTitle", "NewDesc", Guid.NewGuid(), new List<TagDto> { new(tagId, "UpdatedTag") });

            await _service.UpdateForum(request);

            existingForum.Title.ShouldBe("NewTitle");
            existingForum.Description.ShouldBe("NewDesc");
            existingForum.Tags.ShouldContain(tag);
            _forumRepoMock.Verify(r => r.UpdateAsync(existingForum), Times.Once);
        }

        [Fact]
        public async Task UpdateForum_Should_Throw_When_Forum_Not_Found()
        {
            var forumId = Guid.NewGuid();
            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync((Forum?)null);

            var request = new ForumUpdateRequest(forumId, "New", "Desc", Guid.NewGuid(), new());

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.UpdateForum(request));
        }

        [Fact]
        public async Task DeleteForum_Should_Remove_When_Exists()
        {
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var forum = new Forum { Id = forumId, AdminId = userId, Title = "Old", Description = "OldDesc", Tags = new List<Tag>() };

            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User() {Id = userId});
            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync(forum);

            await _service.DeleteForum(forumId, userId);

            _forumRepoMock.Verify(r => r.DeleteAsync(forum), Times.Once);
        }

        [Fact]
        public async Task DeleteForum_Should_Throw_When_Not_Found()
        {
            var forumId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _forumRepoMock.Setup(r => r.GetByIdAsync(forumId)).ReturnsAsync((Forum?)null);

            await Should.ThrowAsync<KeyNotFoundException>(() => _service.DeleteForum(forumId, userId));
        }
    }
}