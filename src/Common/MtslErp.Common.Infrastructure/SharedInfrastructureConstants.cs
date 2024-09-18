namespace MtslErp.Common.Infrastructure;

public static class SharedInfrastructureConstants
{
#pragma warning disable CA2211
    public static TimeSpan DefaultAbsoluteExpiration = TimeSpan.FromMinutes(10);
    public static TimeSpan DefaultSlidingExpiration = TimeSpan.FromMinutes(10);
#pragma warning restore CA2211
}
