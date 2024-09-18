using MtslErp.Common.Domain.Interfaces;
using PrintFactoryManagement.Domain.Orders;
using PrintFactoryManagement.Domain.Repositories;

namespace PrintFactoryManagement.Application;

public interface IPrintFactoryAppUnitOfWork : IUnitOfWork
{
    IOrderRepository OrderRepository { get; }
    IPfmOutboxRepository PfmOutboxRepository { get; }
}
