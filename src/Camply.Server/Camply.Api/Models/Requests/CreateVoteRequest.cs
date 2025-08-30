using System.ComponentModel.DataAnnotations;
using Camply.Shared.Dtos.Vote;

namespace Camply.Api.Models.Requests
{
    public class CreateVoteRequest
    {
        [Required] 
        public string Title { get; set; } = null!;
        
        [Required] 
        public Guid ForumId { get; set; }
        
        [Required] 
        public List<VoteOptionCreateDto> VoteOptions { get; set; } = new();
    }

    public class UpdateVoteRequest
    {
        [Required]
        public string Title { get; set; } = null!;
    }

    public class UpdateVoteOptionRequest
    {
        [Required] 
        public Guid VoteOptionId { get; set; }
        
        [Required] 
        public string Name { get; set; } = null!;
        
        [Required] 
        public int Index { get; set; }
    }
}