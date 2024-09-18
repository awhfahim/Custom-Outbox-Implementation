using MtslErp.Common.Application.Providers;

namespace MtslErp.Common.Infrastructure.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
