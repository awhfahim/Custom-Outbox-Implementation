using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants.UserEntity;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(DbTableName);

        builder.Property(x => x.FullName)
            .HasMaxLength(FullNameMaxLength);

        builder.Property(x => x.UserName)
            .HasMaxLength(UserNameMaxLength);

        builder.Property(x => x.Email)
            .HasMaxLength(EmailMaxLength);

        builder.Property(x => x.ProfilePictureUri)
            .HasMaxLength(ProfilePictureUriMaxLength);

        builder.Property(x => x.Address)
            .HasMaxLength(AddressMaxLength);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(PhoneNumberMaxLength);

        builder.Property(x => x.IsArchived).HasDefaultValue(false);

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.PhoneNumber);
    }
}
