namespace MtslErp.Common.Domain.DataTransferObjects.Response;

public record PagedData<T>(IEnumerable<T> Payload, long TotalCount);
