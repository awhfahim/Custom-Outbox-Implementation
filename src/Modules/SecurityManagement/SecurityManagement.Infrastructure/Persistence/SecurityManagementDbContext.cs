using Humanizer;
using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Interfaces;
using MtslErp.Common.Infrastructure;
using MtslErp.Common.Infrastructure.Persistence.Config;
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ICommonInfrastructureMarker).Assembly);


        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var autoIncrementalEntityInterface = Array.Find(entityType.ClrType.GetInterfaces(), i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAutoIncrementalEntity<>));


            if (autoIncrementalEntityInterface is not null)
            {
                var idProperty = entityType.ClrType.GetProperty("Id");

                if (idProperty is not null)
                {
                    modelBuilder.Entity(entityType.ClrType).Property(idProperty.PropertyType, "Id");
                }
            }

            foreach (var mutableProperty in entityType.GetProperties())
            {
                mutableProperty?.SetColumnName(mutableProperty.Name.Underscore().ToUpper());
            }
        }
    }
}
