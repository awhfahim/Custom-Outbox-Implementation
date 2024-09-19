using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants.UserRoleEntity;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable(DbTableName);

        builder.HasKey(x => new { x.UserId, x.AuthorizableRoleId });

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<AuthorizableRole>()
            .WithMany()
            .HasForeignKey(x => x.AuthorizableRoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
