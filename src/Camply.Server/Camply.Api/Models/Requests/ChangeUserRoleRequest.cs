using Camply.Domain.Enums;

namespace Camply.Api.Models.Requests
{
    public class ChangeUserRoleRequest
    {
        public Guid UserId { get; set; }
        public UserRole Role { get; set; }
    }
}