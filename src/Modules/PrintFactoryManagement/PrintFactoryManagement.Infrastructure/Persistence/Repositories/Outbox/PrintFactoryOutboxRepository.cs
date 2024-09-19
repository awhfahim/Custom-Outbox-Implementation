using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Infrastructure.Persistence.Repositories;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Domain.Repositories.Outbox;

namespace PrintFactoryManagement.Infrastructure.Persistence.Repositories.Outbox;

public class PrintFactoryOutboxRepository : Repository<OutboxMessage>, IPrintFactoryOutboxRepository
{
    public PrintFactoryOutboxRepository(PrintFactoryDbContext context) : base(context)
    {
    }
}
