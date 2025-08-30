using System.ComponentModel.DataAnnotations;
using Camply.Shared.Dtos.Tag;

namespace Camply.Api.Models.Requests
{
    public class UpdateForumRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 1000 characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one tag is required.")]
        public List<string> Tags { get; set; } = new();
    }
}