using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class UserVoteConfiguration : IEntityTypeConfiguration<UserVote>
    {
        public void Configure(EntityTypeBuilder<UserVote> builder)
        {
            builder.Property(x => x.VotedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserVotes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(x => x.Option)
                .WithMany(x => x.UserVotes)
                .HasForeignKey(x => x.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}