using System.Collections.Concurrent;

namespace MtslErp.Common.Domain.CoreProviders;

public interface IReflectionCacheProvider
{
    public ConcurrentDictionary<string, IReadOnlyCollection<(string PropertyName, Type DataType)>>
        DynamicLinqCache { get; }
}
