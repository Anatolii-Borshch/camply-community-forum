using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Security;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.User;
using FluentValidation;

namespace Camply.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        private readonly IValidator<ProfileUpdateRequest> _profileUpdateValidator;
        private readonly IValidator<AccountDeleteRequest> _accountDeleteValidator;
        private readonly IPasswordHasher<User> _passwordHasher;
        
        public UserService(
            IUserRepository userRepository, IValidator<ResetPasswordRequest> resetPasswordValidator
            , IValidator<ProfileUpdateRequest> profileUpdateValidator, IValidator<AccountDeleteRequest> accountDeleteValidator
            , IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _resetPasswordValidator = resetPasswordValidator;
            _profileUpdateValidator = profileUpdateValidator;
            _accountDeleteValidator = accountDeleteValidator;
            _passwordHasher = passwordHasher;
        }
        
        public async Task<UserProfileDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return user.ToUserProfileDto();
        }

        public async Task ChangePasswordAsync(ResetPasswordRequest request)
        {
            await _resetPasswordValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateProfileAsync(ProfileUpdateRequest request)
        {
            await _profileUpdateValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByEmailAsync(request.Username); 
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Name = request.Name;
            user.Surname = request.Surname;
            user.BirthDate = request.Birthday;
            user.Username = request.Username;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAccount(AccountDeleteRequest request)
        {
            await _accountDeleteValidator.ValidateAndThrowAsync(request);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result)
                throw new UnauthorizedAccessException("Invalid password");

            await _userRepository.DeleteAsync(user);
        }
    }
}