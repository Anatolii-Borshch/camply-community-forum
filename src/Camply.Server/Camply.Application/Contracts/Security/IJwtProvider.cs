using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Services
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}