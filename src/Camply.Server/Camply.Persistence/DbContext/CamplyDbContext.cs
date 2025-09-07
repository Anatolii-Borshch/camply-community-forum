using Camply.Domain.Entities;
using Camply.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.DbContext
{
    public class CamplyDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public CamplyDbContext(DbContextOptions<CamplyDbContext> options) : base(options){}

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteOption> VoteOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CamplyDbContext).Assembly);
            
            //Below predefined system administrator
            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("D9C6EBA2-E12D-47F4-AACA-B16DE7478083"),
                CreatedDate = DateTime.UtcNow,
                Name = "Camply",
                Surname = "Official",
                Username = "camply_official",
                Email = "camply@gmail.com",
                PasswordHash = "10000.QiRQG5c2S/3oxJTAZKiQsQ==.c9R4j4Rp+C6h+RTla3gTGLxQE8ygG9kcGAHYGy9eMGs=", //Password: 123123ab
                Role = UserRole.Administrator,
                BirthDate = DateTime.UtcNow,
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}