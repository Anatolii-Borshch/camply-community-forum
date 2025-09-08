using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class VoteOptionConfiguration : IEntityTypeConfiguration<VoteOption>
    {
        public void Configure(EntityTypeBuilder<VoteOption> builder)
        {
            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.ModifiedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();
            
            builder.HasOne(x => x.Vote)
                .WithMany(x => x.VoteOptions)
                .HasForeignKey(x => x.VoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}