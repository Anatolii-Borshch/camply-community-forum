using System.Security.Claims;
using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Security;
using Camply.Application.Contracts.Services;
using Camply.Application.Security;
using Camply.Domain.Entities;
using Camply.Domain.Enums;
using Camply.Shared.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Camply.Security.PrivacyServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        
        public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider
            , IPasswordHasher<User> passwordHasher, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Guid UserId 
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
            }
        }

        public async Task<LoginData> Login(UserLoginDto userLoginDto)
        {
            _logger.LogInformation("Attempting login for user: {EmailOrUsername}", userLoginDto.Email ?? userLoginDto.Username);
            User? user = null;

            if (!string.IsNullOrEmpty(userLoginDto.Email))
            {
                user = await _userRepository.GetByEmailAsync(userLoginDto.Email);
            }
            else if (!string.IsNullOrEmpty(userLoginDto.Username))
            {
                user = await _userRepository.GetByUsernameAsync(userLoginDto.Username);
            }
            
            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for {EmailOrUsername}", userLoginDto.Email ?? userLoginDto.Username);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var validPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);
            
            if (!validPassword)
            {
                _logger.LogWarning("Login failed: invalid password for user {UserId}", user.Id);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var token = _jwtProvider.GenerateToken(user);
            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            
            return new LoginData
            {
                UserId = user.Id,
                Token = token,
            };
        }

        public async Task Register(UserRegisterDto userData)
        {
            _logger.LogInformation("Registering new user with email {Email}", userData.Email);
            var users = await _userRepository.GetAllAsync();
            
            var existingUser = users.FirstOrDefault(u => u.Email == userData.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: user with email {Email} already exists", userData.Email);
                throw new InvalidOperationException("User already exists.");
            }

            var user = new User
            {
                Name = userData.Name,
                Surname = userData.Surname,
                Username = userData.Username,
                Role = UserRole.Poster,
                Email = userData.Email,
                BirthDate = userData.BirthDate,
                CreatedDate = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userData.Password);
            
            await _userRepository.AddAsync(user);
            _logger.LogInformation("User {UserId} registered successfully", user.Id);
        }

        public async Task Logout()
        {
            _logger.LogInformation("User {UserId} logged out", UserId);
            await Task.CompletedTask;
        }
    }
}