using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Domain.Repositories;

namespace PrintFactoryManagement.Domain.Repositories.Outbox;

public interface IPrintFactoryOutboxRepository : IRepositoryBase<OutboxMessage>
{

}
