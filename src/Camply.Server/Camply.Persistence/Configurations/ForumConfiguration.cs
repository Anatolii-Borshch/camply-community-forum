using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class ForumConfiguration : IEntityTypeConfiguration<Forum>
    {
        public void Configure(EntityTypeBuilder<Forum> builder)
        {
            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();
            
            builder.HasOne(x => x.Admin)
                .WithMany(x => x.Forums)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}