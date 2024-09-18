namespace MtslErp.Common.Application.Providers;

public interface IGuidProvider
{
    Guid SortableGuid();
    Guid RandomGuid();
}
