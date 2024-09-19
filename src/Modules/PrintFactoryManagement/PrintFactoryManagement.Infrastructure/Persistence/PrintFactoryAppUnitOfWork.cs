using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Orders;
using PrintFactoryManagement.Domain.Repositories;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PrintFactoryAppUnitOfWork : PrintFactoryUnitOfWork, IPrintFactoryAppUnitOfWork
{
    public PrintFactoryAppUnitOfWork(PrintFactoryDbContext dbContext,
        IOrderRepository orderRepository, IPfmOutboxRepository pfmOutboxRepository) : base(dbContext)
    {
        OrderRepository = orderRepository;
        PfmOutboxRepository = pfmOutboxRepository;
    }
    public IOrderRepository OrderRepository { get; }
    public IPfmOutboxRepository PfmOutboxRepository { get; }
}
