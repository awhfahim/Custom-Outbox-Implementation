using System.Collections.Concurrent;
using MtslErp.Common.Domain.CoreProviders;

namespace MtslErp.Common.Infrastructure.Providers;

public class ReflectionCacheProvider : IReflectionCacheProvider
{
    public ConcurrentDictionary<string, IReadOnlyCollection<(string PropertyName, Type DataType)>>
        DynamicLinqCache { get; } = new();
}
