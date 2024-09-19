using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Domain.Repositories;

namespace SecurityManagement.Domain.Repositories.Outbox;

public interface ISecurityManagementOutboxRepository : IRepositoryBase<OutboxMessage>
{

}
