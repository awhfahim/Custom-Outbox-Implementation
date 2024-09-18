using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Infrastructure.Persistence.Repositories;
using PrintFactoryManagement.Domain.Repositories;

namespace PrintFactoryManagement.Infrastructure.Persistence.Repositories;

public class PfmOutboxRepository : Repository<OutboxMessage, int>, IPfmOutboxRepository
{
    public PfmOutboxRepository(PfmDbContext context) : base(context)
    {
    }
}
