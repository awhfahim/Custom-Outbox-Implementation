using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Interfaces;
using SecurityManagement.Domain.Entities;

namespace SecurityManagement.Infrastructure.Persistence;

public class SecurityManagementDbContext(DbContextOptions<SecurityManagementDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AuthorizableRole> AuthorizableRoles => Set<AuthorizableRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuthorizablePermission> AuthorizablePermissions => Set<AuthorizablePermission>();

    public DbSet<AuthorizablePermissionGroup> AuthorizablePermissionGroups =>
        Set<AuthorizablePermissionGroup>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SecurityManagementInfrastructureConstants.DbSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecurityManagementDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var autoIncrementalEntityInterface = clrType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType
                                     && i.GetGenericTypeDefinition() == typeof(IAutoIncrementalEntity<>));

            if (autoIncrementalEntityInterface is null)
            {
                continue;
            }

            var idProperty = clrType.GetProperty("Id");

            if (idProperty is not null)
            {
                modelBuilder.Entity(clrType).Property(idProperty.PropertyType, "Id");
            }
        }
    }
}
