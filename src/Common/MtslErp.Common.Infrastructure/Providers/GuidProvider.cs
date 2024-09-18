using MtslErp.Common.Application.Providers;

namespace MtslErp.Common.Infrastructure.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid SortableGuid() => Ulid.NewUlid().ToGuid();

    public Guid RandomGuid() => Guid.NewGuid();
}
