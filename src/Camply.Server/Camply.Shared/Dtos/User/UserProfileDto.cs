using Camply.Shared.Dtos.Forum;

namespace Camply.Shared.Dtos.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public int CreatedPostAmount { get; set; }
        public List<ForumPrevievDto> CreatedForums { get; set; } = new List<ForumPrevievDto>();
    }
}