using Camply.Domain.Enums;

namespace Camply.Api.Models.Helpers
{
    
    public static class RoleHelper
    {
        public const string Admin = nameof(UserRole.Administrator);
        public const string Poster = nameof(UserRole.Poster);
    }
}