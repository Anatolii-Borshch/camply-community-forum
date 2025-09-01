using System.ComponentModel.DataAnnotations;
using Camply.Shared.Dtos.Tag;

namespace Camply.Api.Models.Requests
{
    public class CreateForumRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description must be between 5 and 1000 characters.")]
        public string? Description { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one tag is required.")]
        public List<Guid> Tags { get; set; } = new();
    }
}