using Camply.Domain.Enums;

namespace Camply.Api.Models.Requests
{
    public class ChangeUserRoleRequest
    {
        public UserRole Role { get; set; }
    }
}