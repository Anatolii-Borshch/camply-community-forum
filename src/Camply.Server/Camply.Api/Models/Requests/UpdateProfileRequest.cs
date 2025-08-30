using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class UpdateProfileRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public string Username { get; set; } = null!;
    }
}