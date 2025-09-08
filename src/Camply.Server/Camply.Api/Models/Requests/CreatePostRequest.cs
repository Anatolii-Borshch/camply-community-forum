using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class CreatePostRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        
        public string? Description { get; set; }
        
        [Required]
        public Guid ForumId { get; set; }
    }
}