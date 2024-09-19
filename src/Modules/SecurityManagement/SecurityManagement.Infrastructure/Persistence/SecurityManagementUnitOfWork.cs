using MtslErp.Common.Infrastructure.Persistence;

namespace SecurityManagement.Infrastructure.Persistence;

public abstract class SecurityManagementUnitOfWork : UnitOfWorkBase
{
    protected SecurityManagementUnitOfWork(SecurityManagementDbContext dbContext) : base(dbContext)
    {
    }
}
