using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            
            builder.Property(x => x.ModifiedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            
            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(x => x.Forum)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.ForumId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(x => x.User)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasMany(x => x.LikedPosts)
                .WithOne(lp => lp.Post)
                .HasForeignKey(lp => lp.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SavedPosts)
                .WithOne(sp => sp.Post)
                .HasForeignKey(sp => sp.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}