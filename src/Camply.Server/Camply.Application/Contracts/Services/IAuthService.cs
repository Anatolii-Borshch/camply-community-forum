using Camply.Application.Security;
using Camply.Shared.Dtos;

namespace Camply.Application.Contracts.Services
{
    public interface IAuthService
    {
        public Guid UserId { get; }
        Task<LoginData> Login(UserLoginDto userLoginData);
        Task Register(UserRegisterDto userData);
        Task Logout();
    }
}