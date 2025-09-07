using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class UpdateVoteRequest
    {
        [Required]
        public string Title { get; set; } = null!;
    }
}