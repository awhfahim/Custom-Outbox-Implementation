namespace MtslErp.Common.Application.Providers;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}

