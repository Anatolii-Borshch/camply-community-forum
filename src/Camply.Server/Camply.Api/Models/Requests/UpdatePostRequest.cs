using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class UpdatePostRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        
        public string? Description { get; set; }
    }
}