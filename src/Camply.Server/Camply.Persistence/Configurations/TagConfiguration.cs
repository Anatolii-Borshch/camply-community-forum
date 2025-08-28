using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.HasMany<Forum>()
                .WithMany(x => x.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "ForumTags",
                    x => x.HasOne<Forum>()
                        .WithMany()
                        .HasForeignKey("ForumId")
                        .OnDelete(DeleteBehavior.Cascade),
                    x => x.HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                    x =>
                    {
                        x.HasKey("ForumId", "TagId");
                        x.ToTable("ForumTags");
                    });
        }
    }
}