using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class DeleteAccountRequest
    {
        [Required]
        public string Password { get; set; } = null!;
    }
}