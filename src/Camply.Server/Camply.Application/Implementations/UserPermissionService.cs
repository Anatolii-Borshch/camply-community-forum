using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public abstract class UserPermissionService
    {
        private readonly IUserRepository _userRepository;
        
        private readonly ILogger _logger;
        
        public UserPermissionService(IUserRepository userRepository, ILogger logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        
        public async Task<User> EnsureUserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"User with id {userId} not found");
                throw new KeyNotFoundException($"User with id {userId} not found.");
            }

            return user;
        }

        public void EnsureUserAcess(User user, Guid? userId)
        {
            if (user.Role == UserRole.Administrator)
                return;

            if (userId != user.Id)
            {
                _logger.LogWarning($"User with id {userId} does not have permission.");
                throw new UnauthorizedAccessException("User does not have permission.");
            }
        }
    }
}