using Camply.Shared.Dtos.User;

namespace Camply.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserByIdAsync(Guid userId);
        Task ChangePasswordAsync(ResetPasswordRequest request);
        Task UpdateProfileAsync(ProfileUpdateRequest request);
        Task DeleteAccount(AccountDeleteRequest request);
    }
}