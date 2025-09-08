using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class LoginRequest
    {
        [StringLength(30, ErrorMessage = "Username must be less than 30 characters")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}