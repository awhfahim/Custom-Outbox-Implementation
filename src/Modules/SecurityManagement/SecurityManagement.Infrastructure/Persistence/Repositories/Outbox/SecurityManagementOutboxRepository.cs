using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Infrastructure.Persistence.Repositories;
using SecurityManagement.Domain.Repositories.Outbox;

namespace SecurityManagement.Infrastructure.Persistence.Repositories.Outbox;

public class SecurityManagementOutboxRepository : Repository<OutboxMessage>, ISecurityManagementOutboxRepository
{
    public SecurityManagementOutboxRepository(SecurityManagementDbContext context) : base(context)
    {
    }
}
