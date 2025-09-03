using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Security;
using Camply.Application.Implementations;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.User;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IValidator<ResetPasswordRequest>> _resetPasswordValidator = new();
        private readonly Mock<IValidator<ProfileUpdateRequest>> _profileUpdateValidator = new();
        private readonly Mock<IValidator<AccountDeleteRequest>> _accountDeleteValidator = new();
        private readonly Mock<IPasswordHasher<User>> _passwordHasher  = new();
        private readonly UserService _service;
        private readonly IMemoryCache _cache;
        
        public UserServiceTests()
        {
            _resetPasswordValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ResetPasswordRequest>(), default))
                .ReturnsAsync(new ValidationResult());

            _profileUpdateValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ProfileUpdateRequest>(), default))
                .ReturnsAsync(new ValidationResult());

            _accountDeleteValidator
                .Setup(v => v.ValidateAsync(It.IsAny<AccountDeleteRequest>(), default))
                .ReturnsAsync(new ValidationResult()); 

            _cache = new MemoryCache(new MemoryCacheOptions());
            
            _service = new UserService(
                _userRepoMock.Object,
                _resetPasswordValidator.Object,
                _profileUpdateValidator.Object,
                _accountDeleteValidator.Object,
                _passwordHasher.Object,
                NullLogger<UserService>.Instance,
                _cache
                );
        }

        [Fact]
        public async Task GetUserByIdAsync_Should_Return_UserProfileDto()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "John" };
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync(userId);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(userId);
            result.Name.ShouldBe("John");
        }

        [Fact]
        public async Task GetUserByIdAsync_Should_Throw_When_Not_Found()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => _service.GetUserByIdAsync(id));
            ex.Message.ShouldBe($"User with id {id} not found.");
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Update_Hash()
        {
            var user = new User { Id = Guid.NewGuid(), PasswordHash = "oldhash" };
            var request = new ResetPasswordRequest(user.Id, "newpass");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _passwordHasher.Setup(h => h.HashPassword(user, "newpass")).Returns("newhash");

            await _service.ChangePasswordAsync(request);

            user.PasswordHash.ShouldBe("newhash");
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_When_User_Not_Found()
        {
            var request = new ResetPasswordRequest(Guid.NewGuid(), "pass");
            _userRepoMock.Setup(r => r.GetByIdAsync(request.UserId)).ReturnsAsync((User?)null);

            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => _service.ChangePasswordAsync(request));
            ex.Message.ShouldBe($"User with id {request.UserId} not found.");
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_Update_Fields()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "Old" };
            var request = new ProfileUpdateRequest(user.Id, "New", "Surname", DateTime.UtcNow, "username");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            await _service.UpdateProfileAsync(request);

            user.Name.ShouldBe("New");
            user.Surname.ShouldBe("Surname");
            user.Username.ShouldBe("username");
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_Throw_When_User_Not_Found()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "Old" };
            var request = new ProfileUpdateRequest(user.Id, "N", "S", DateTime.UtcNow, "u");
            _userRepoMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((User?)null);

            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => _service.UpdateProfileAsync(request));
            ex.Message.ShouldBe($"User with id {user.Id} not found.");
        }

        [Fact]
        public async Task DeleteAccount_Should_Delete_When_Password_Matches()
        {
            var user = new User { Id = Guid.NewGuid(), PasswordHash = "hash" };
            var request = new AccountDeleteRequest(user.Id, "pass");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _passwordHasher.Setup(h => h.VerifyHashedPassword(user, "hash", "pass")).Returns(true);

            await _service.DeleteAccount(request);

            _userRepoMock.Verify(r => r.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAccount_Should_Throw_When_User_Not_Found()
        {
            var request = new AccountDeleteRequest(Guid.NewGuid(), "pass");
            _userRepoMock.Setup(r => r.GetByIdAsync(request.UserId)).ReturnsAsync((User?)null);

            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => _service.DeleteAccount(request));
            ex.Message.ShouldBe($"User with id {request.UserId} not found.");
        }

        [Fact]
        public async Task DeleteAccount_Should_Throw_When_Password_Invalid()
        {
            var user = new User { Id = Guid.NewGuid(), PasswordHash = "hash" };
            var request = new AccountDeleteRequest(user.Id, "wrong");

            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _passwordHasher.Setup(h => h.VerifyHashedPassword(user, "hash", "wrong")).Returns(false);

            var ex = await Should.ThrowAsync<UnauthorizedAccessException>(() => _service.DeleteAccount(request));
            ex.Message.ShouldBe("Invalid password");

            _userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
    }
}