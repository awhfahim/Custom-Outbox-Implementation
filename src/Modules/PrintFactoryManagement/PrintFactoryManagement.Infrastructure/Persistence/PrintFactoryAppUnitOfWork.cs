using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Domain.Repositories.Outbox;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PrintFactoryAppUnitOfWork : PrintFactoryUnitOfWork, IPrintFactoryAppUnitOfWork
{
    public PrintFactoryAppUnitOfWork(PrintFactoryDbContext dbContext,
        IPrintFactoryOutboxRepository printFactoryOutboxRepository)
        : base(dbContext)
    {
        PrintFactoryOutboxRepository = printFactoryOutboxRepository;
    }

    public IPrintFactoryOutboxRepository PrintFactoryOutboxRepository { get; }
}
