using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;
using static Domain.SecurityManagementDomainConstants;

public class AuthorizablePermissionGroupConfig : IEntityTypeConfiguration<AuthorizablePermissionGroup>
{
    public void Configure(EntityTypeBuilder<AuthorizablePermissionGroup> builder)
    {
        builder.ToTable(AuthorizablePermissionEntity.DbTableName);
        builder.Property(x => x.Label).HasMaxLength(AuthorizablePermissionEntity.LabelMaxLength);
        builder.HasIndex(x => x.Label).IsUnique();
    }
}
