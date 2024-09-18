using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(UserEntity.DbTableName);

        builder.Property(x => x.FullName)
            .HasMaxLength(UserEntity.FullNameMaxLength);

        builder.Property(x => x.UserName)
            .HasMaxLength(UserEntity.UserNameMaxLength);

        builder.Property(x => x.Email)
            .HasMaxLength(UserEntity.EmailMaxLength);

        builder.Property(x => x.ProfilePictureUri)
            .HasMaxLength(UserEntity.ProfilePictureUriMaxLength);

        builder.Property(x => x.Address)
            .HasMaxLength(UserEntity.AddressMaxLength);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(UserEntity.PhoneNumberMaxLength);

        builder.Property(x => x.IsArchived).HasDefaultValue(false);

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.PhoneNumber);
    }
}
