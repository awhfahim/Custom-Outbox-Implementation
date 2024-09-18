using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class AuthorizablePermissionConfig : IEntityTypeConfiguration<AuthorizablePermission>
{
    public void Configure(EntityTypeBuilder<AuthorizablePermission> builder)
    {
        builder.ToTable(AuthorizablePermissionEntity.DbTableName);
        builder.Property(x => x.Label).HasMaxLength(AuthorizablePermissionEntity.LabelMaxLength);
        builder.HasIndex(x => x.Label).IsUnique();

        builder
            .HasOne<AuthorizablePermissionGroup>()
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("Fk_AuthPermissions_Category");
    }
}
