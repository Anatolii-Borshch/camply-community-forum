using Camply.Domain.Entities;
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
            
            base.OnModelCreating(modelBuilder);
        }
    }
}