using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string Password { get; set; } = null!;
    }
}