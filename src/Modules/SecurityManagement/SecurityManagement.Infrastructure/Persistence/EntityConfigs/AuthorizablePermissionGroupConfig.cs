using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

using static Domain.SecurityManagementDomainConstants.AuthorizablePermissionGroupEntity;

public class AuthorizablePermissionGroupConfig : IEntityTypeConfiguration<AuthorizablePermissionGroup>
{
    public void Configure(EntityTypeBuilder<AuthorizablePermissionGroup> builder)
    {
        builder.ToTable(DbTableName.Underscore().ToUpper());
        builder.Property(x => x.Label).HasMaxLength(LabelMaxLength);
        builder.HasIndex(x => x.Label).IsUnique();
    }
}
