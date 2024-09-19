using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Interfaces;
using MtslErp.Common.Infrastructure.Persistence.Config;
using PrintFactoryManagement.Domain.Entities;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PrintFactoryDbContext(DbContextOptions<PrintFactoryDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrintFactoryDbContext).Assembly);

        modelBuilder.HasDefaultSchema("PFM");
        modelBuilder.ApplyConfiguration(new OutboxMessageConfig());
        modelBuilder.ApplyConfiguration(new InboxStateConfig());

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
