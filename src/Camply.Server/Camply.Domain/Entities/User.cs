using Camply.Domain.Enums;

namespace Camply.Domain.Entities
{
    public class User : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime BirthDate { get; set; }
        
        public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        public virtual ICollection<LikedPost> LikedPosts { get; set; } = new List<LikedPost>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<UserVote> UserVotes { get; set; } = new List<UserVote>();
        public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
    }
}