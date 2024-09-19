using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecurityManagement.Domain.Entities;
using static SecurityManagement.Domain.SecurityManagementDomainConstants.AuthorizableRoleEntity;

namespace SecurityManagement.Infrastructure.Persistence.EntityConfigs;

public class AuthorizableRoleConfig : IEntityTypeConfiguration<AuthorizableRole>
{
    public void Configure(EntityTypeBuilder<AuthorizableRole> builder)
    {
        builder.ToTable(DbTableName);
        builder.Property(x => x.Label).HasMaxLength(LabelMaxLength);
        builder.HasIndex(x => x.Label).IsUnique();
    }
}
