using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Infrastructure.Persistence.config;
using PrintFactoryManagement.Domain.Orders;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PfmDbContext(DbContextOptions<PfmDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("PFM");
        modelBuilder.ApplyConfiguration(new OutboxMessageConfig());
    }
}
