using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Domain.Repositories.Outbox;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public class PrintFactoryAppUnitOfWork : PrintFactoryUnitOfWork, IPrintFactoryAppUnitOfWork
{
    public PrintFactoryAppUnitOfWork(PrintFactoryDbContext dbContext,
        IPrintFactoryOutboxRepository printFactoryOutboxRepository,
        IUserRepository userRepository) : base(dbContext)
    {
        PrintFactoryOutboxRepository = printFactoryOutboxRepository;
        UserRepository = userRepository;
    }

    public IPrintFactoryOutboxRepository PrintFactoryOutboxRepository { get; }
    public IUserRepository UserRepository { get; }
}
