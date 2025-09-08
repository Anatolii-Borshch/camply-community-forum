using Camply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camply.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.Surname)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.Username)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(x => x.Email)
                .HasMaxLength(70)
                .IsRequired();
            
            builder.Property(x => x.Role)
                .HasConversion<string>()
                .IsRequired();
            
            builder.Property(x => x.BirthDate)
                .IsRequired();
        }
    }
}