using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Orders;
using PrintFactoryManagement.Domain.Repositories;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PfmAppUnitOfWork : PfmUnitOfWork, IPrintFactoryAppUnitOfWork
{
    public PfmAppUnitOfWork(PfmDbContext dbContext,
        IOrderRepository orderRepository, IPfmOutboxRepository pfmOutboxRepository) : base(dbContext)
    {
        OrderRepository = orderRepository;
        PfmOutboxRepository = pfmOutboxRepository;
    }
    public IOrderRepository OrderRepository { get; }
    public IPfmOutboxRepository PfmOutboxRepository { get; }
}
