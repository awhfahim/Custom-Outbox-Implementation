using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Domain.Repositories;

namespace PrintFactoryManagement.Domain.Repositories;

public interface IPfmOutboxRepository : IRepositoryBase<OutboxMessage>
{

}
