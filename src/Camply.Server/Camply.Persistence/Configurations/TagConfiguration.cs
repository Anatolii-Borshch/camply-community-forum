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
            
            builder.HasMany(t => t.Forums)
                .WithMany(f => f.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "ForumTags",
                    j => j.HasOne<Forum>()
                        .WithMany()
                        .HasForeignKey("ForumId")
                        .HasConstraintName("FK_ForumTags_Forum")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_ForumTags_Tag")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("ForumId", "TagId");
                        j.ToTable("ForumTags");
                    });
        }
    }
}