using System.Security.Claims;
using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Security;
using Camply.Domain.Entities;
using Camply.Shared.Dtos;
using Camply.Shared.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Camply.Security.PrivacyServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider
            , IPasswordHasher<User> passwordHasher, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
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
                throw new UnauthorizedAccessException("Invalid credentials.");

            var validPassword = _passwordHasher.VerifyHashedPassword(user ,user.PasswordHash, userLoginDto.Password);
            
            if (validPassword == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _jwtProvider.GenerateToken(user);
            
            return new LoginData
            {
                UserId = user.Id,
                Token = token,
            };
        }

        public async Task Register(UserRegisterDto userData)
        {
            var users = await _userRepository.GetAllAsync();
            var existingUser = users.FirstOrDefault(u => u.Email == userData.Email);
            
            if (existingUser != null)
                throw new InvalidOperationException("User already exists.");

            var user = new User
            {
                Name = userData.Name,
                Surname = userData.Surname,
                Username = userData.Username,
                Email = userData.Email,
                BirthDate = userData.BirthDate,
                CreatedDate = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userData.Password);
            
            await _userRepository.AddAsync(user);
        }

        public async Task Logout()
        {
            await Task.CompletedTask;
        }
    }
}