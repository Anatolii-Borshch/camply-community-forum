using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Domain.Entities;
using Camply.Domain.Enums;
using Camply.Shared.Dtos.Tag;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class TagServiceTests
    {
        private readonly Mock<ITagRepository> _tagRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly TagService _service;

        public TagServiceTests()
        {
            _service = new TagService
                (_tagRepoMock.Object
                    , NullLogger<TagService>.Instance
                    , _userRepoMock.Object);
        }

        [Fact]
        public async Task GetTagsAsync_Should_Return_All_Tags()
        {
            var tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "test1" },
                new() { Id = Guid.NewGuid(), Name = "test2" }
            };
            _tagRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tags);

            var result = await _service.GetTagsAsync();

            var tagDtos = result.ToList();
            tagDtos.ShouldNotBeEmpty();
            tagDtos.Count().ShouldBe(2);
            tagDtos.ShouldContain(t => t.Name == "test1");
            tagDtos.ShouldContain(t => t.Name == "test2");
        }

        [Fact]
        public async Task AddTagAsync_Should_Add_When_Not_Exists()
        {
            var userId = Guid.NewGuid();
            
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User() { Role = UserRole.Administrator});
            _tagRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tag>());

            await _service.AddTagAsync("newtag", userId);

            _tagRepoMock.Verify(r => r.AddAsync(It.Is<Tag>(t => t.Name == "newtag")), Times.Once);
        }

        [Fact]
        public async Task AddTagAsync_Should_Throw_When_Duplicate_Exists()
        {
            var existing = new Tag { Id = Guid.NewGuid(), Name = "duplicate" };
            
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User() { Role = UserRole.Administrator});
            _tagRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tag> { existing });

            var ex = await Should.ThrowAsync<ArgumentException>(() => _service.AddTagAsync("duplicate", Guid.NewGuid()));
            
            ex.Message.ShouldBe("Tag already exists");
            _tagRepoMock.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTagAsync_Should_Update_When_Valid()
        {
            var id = Guid.NewGuid();
            var tag = new Tag { Id = id, Name = "old" };
            var user = new User { Id = Guid.NewGuid(), Name = "newuser", Role = UserRole.Administrator};
            
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _tagRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(tag);
            _tagRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tag> { tag });

            var request = new TagUpdateRequest(id, "new", user.Id);

            await _service.UpdateTagAsync(request);

            tag.Name.ShouldBe("new");
            
            _tagRepoMock.Verify(r => r.UpdateAsync(tag), Times.Once);
        }

        [Fact]
        public async Task UpdateTagAsync_Should_Throw_When_Not_Found()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "newuser", Role = UserRole.Administrator };
            var request = new TagUpdateRequest(Guid.NewGuid(), "new", user.Id);
            
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _tagRepoMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((Tag?)null);

            var ex = await Should.ThrowAsync<ArgumentException>(() => _service.UpdateTagAsync(request));

            ex.Message.ShouldBe("Tag not found");
        }

        [Fact]
        public async Task UpdateTagAsync_Should_Throw_When_Duplicate_Name()
        {
            var id = Guid.NewGuid();
            var tag = new Tag { Id = id, Name = "old" };
            var duplicate = new Tag { Id = Guid.NewGuid(), Name = "duplicate" };
            var user = new User { Id = Guid.NewGuid(), Name = "newuser", Role = UserRole.Administrator };

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _tagRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(tag);
            _tagRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tag> { tag, duplicate });

            var request = new TagUpdateRequest(id, "duplicate", user.Id);

            var ex = await Should.ThrowAsync<ArgumentException>(() => _service.UpdateTagAsync(request));

            ex.Message.ShouldBe("Tag already exists");
        }

        [Fact]
        public async Task DeleteTagAsync_Should_Delete_When_Exists()
        {
            var id = Guid.NewGuid();
            var tag = new Tag { Id = id, Name = "todelete" };
            
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User() { Role = UserRole.Administrator });
            _tagRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(tag);

            await _service.DeleteTagAsync(id, Guid.NewGuid());

            _tagRepoMock.Verify(r => r.DeleteAsync(tag), Times.Once);
        }

        [Fact]
        public async Task DeleteTagAsync_Should_Throw_When_Not_Found()
        {
            var id = Guid.NewGuid();
            _tagRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Tag?)null);

            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new User() { Role = UserRole.Administrator });
            
            var ex = await Should.ThrowAsync<ArgumentException>(() => _service.DeleteTagAsync(id, Guid.NewGuid()));
            ex.Message.ShouldBe("Tag not found");

            _tagRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Tag>()), Times.Never);
        }
    }
}