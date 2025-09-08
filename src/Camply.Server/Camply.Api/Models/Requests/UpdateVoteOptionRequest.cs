using System.ComponentModel.DataAnnotations;

namespace Camply.Api.Models.Requests
{
    public class UpdateVoteOptionRequest
    {
        
        [Required] 
        public string Name { get; set; } = null!;
        
        [Required] 
        public int Index { get; set; }
    }
}