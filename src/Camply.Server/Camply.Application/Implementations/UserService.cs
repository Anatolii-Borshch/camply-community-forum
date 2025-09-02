using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Security;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.User;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        private readonly IValidator<ProfileUpdateRequest> _profileUpdateValidator;
        private readonly IValidator<AccountDeleteRequest> _accountDeleteValidator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        
        public UserService(
            IUserRepository userRepository, IValidator<ResetPasswordRequest> resetPasswordValidator
            , IValidator<ProfileUpdateRequest> profileUpdateValidator, IValidator<AccountDeleteRequest> accountDeleteValidator
            , IPasswordHasher<User> passwordHasher, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _resetPasswordValidator = resetPasswordValidator;
            _profileUpdateValidator = profileUpdateValidator;
            _accountDeleteValidator = accountDeleteValidator;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        
        public async Task<UserProfileDto> GetUserByIdAsync(Guid userId)
        {
            _logger.LogInformation("Fetching user by id {UserId}", userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new KeyNotFoundException("User not found");
            }

            _logger.LogInformation("User {UserId} fetched successfully", userId);
            return user.ToUserProfileDto();
        }

        public async Task ChangePasswordAsync(ResetPasswordRequest request)
        {
            _logger.LogInformation("Changing password for user {UserId}", request.UserId);
            await _resetPasswordValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for password change", request.UserId);
                throw new KeyNotFoundException("User not found");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation("Password changed successfully for user {UserId}", request.UserId);
        }

        public async Task UpdateProfileAsync(ProfileUpdateRequest request)
        {
            _logger.LogInformation("Updating profile for user {UserId}", request.Id);
            await _profileUpdateValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByIdAsync(request.Id); 
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for profile update", request.Id);
                throw new KeyNotFoundException("User not found");
            }

            user.Name = request.Name;
            user.Surname = request.Surname;
            user.BirthDate = request.Birthday;
            user.Username = request.Username;

            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation("Profile updated successfully for user {UserId}", request.Id);
        }

        public async Task DeleteAccount(AccountDeleteRequest request)
        {
            _logger.LogInformation("Deleting account for user {UserId}", request.UserId);
            await _accountDeleteValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", request.UserId);
                throw new KeyNotFoundException("User not found");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (!result)
            {
                _logger.LogWarning("Invalid password provided for account deletion of user {UserId}", request.UserId);
                throw new UnauthorizedAccessException("Invalid password");
            }

            await _userRepository.DeleteAsync(user);
            
            _logger.LogInformation("Account deleted successfully for user {UserId}", request.UserId);
        }
    }
}