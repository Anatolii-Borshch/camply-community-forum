using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class CreateCommentRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Content { get; set; } = null!;
        [Required]
        public Guid PostId { get; set; }
        
        public Guid? ParentCommentId { get; set; }
    }
}