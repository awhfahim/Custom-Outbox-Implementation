using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Oracle.EntityFrameworkCore.Metadata;
using PrintFactoryManagement.Domain.Orders;

namespace PrintFactoryManagement.Infrastructure.Persistence.Configs;

public class OrderEntityConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

    }
}
