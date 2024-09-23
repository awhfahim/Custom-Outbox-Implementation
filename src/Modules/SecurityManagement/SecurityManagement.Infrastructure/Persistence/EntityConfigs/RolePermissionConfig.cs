using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants.RolePermissionEntity;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(DbTableName.Underscore().ToUpper());

        builder.HasKey(x => new { x.AuthorizableRoleId, x.AuthorizablePermissionId });

        builder
            .HasOne<AuthorizableRole>()
            .WithMany()
            .HasForeignKey(x => x.AuthorizableRoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("Fk_RolePermissions_AuthRole");

        builder
            .HasOne<AuthorizablePermission>()
            .WithMany()
            .HasForeignKey(x => x.AuthorizablePermissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("Fk_RolePermissions_AuthPermission");
    }
}
