using MtslErp.Common.Domain.Interfaces;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Domain.Repositories.Outbox;

namespace PrintFactoryManagement.Application;

public interface IPrintFactoryAppUnitOfWork : IUnitOfWork
{
    IPrintFactoryOutboxRepository PrintFactoryOutboxRepository { get; }
    IUserRepository UserRepository { get; }
}
