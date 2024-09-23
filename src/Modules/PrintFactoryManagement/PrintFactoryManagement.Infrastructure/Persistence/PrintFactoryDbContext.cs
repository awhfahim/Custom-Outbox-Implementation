using Humanizer;
using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Interfaces;
using MtslErp.Common.Infrastructure;
using MtslErp.Common.Infrastructure.Persistence.Config;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PrintFactoryDbContext(DbContextOptions<PrintFactoryDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrintFactoryDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ICommonInfrastructureMarker).Assembly);

        modelBuilder.HasDefaultSchema("PFM");

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
