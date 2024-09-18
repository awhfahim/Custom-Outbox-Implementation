using MtslErp.Common.Infrastructure.Persistence.Repositories;
using PrintFactoryManagement.Domain.Orders;

namespace PrintFactoryManagement.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order, int>, IOrderRepository
{
    public OrderRepository(PfmDbContext context) : base(context)
    {
    }
}
