using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.ModifiedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            
            builder.Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();
            
            builder.HasOne(x => x.Forum)
                .WithMany(x => x.Votes)
                .HasForeignKey(x => x.ForumId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(x => x.User)
                .WithMany(x => x.Votes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}