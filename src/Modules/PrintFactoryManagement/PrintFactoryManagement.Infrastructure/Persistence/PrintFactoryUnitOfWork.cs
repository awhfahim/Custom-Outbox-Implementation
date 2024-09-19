using MtslErp.Common.Infrastructure.Persistence;

namespace PrintFactoryManagement.Infrastructure.Persistence;

public abstract class PrintFactoryUnitOfWork : UnitOfWorkBase
{
    protected PrintFactoryUnitOfWork(PrintFactoryDbContext dbContext) : base(dbContext)
    {
    }
}
