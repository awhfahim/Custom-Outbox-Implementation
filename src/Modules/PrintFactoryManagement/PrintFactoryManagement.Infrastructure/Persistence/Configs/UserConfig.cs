using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintFactoryManagement.Domain.Entities;

namespace PrintFactoryManagement.Infrastructure.Persistence.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.FullName)
            .HasMaxLength(50);

        builder.Property(x => x.UserName)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(50);

        builder.Property(x => x.ProfilePictureUri)
            .HasMaxLength(50);

        builder.Property(x => x.Address)
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(15);

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.PhoneNumber);
    }
}
