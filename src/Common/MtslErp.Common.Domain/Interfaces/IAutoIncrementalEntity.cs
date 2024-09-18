namespace MtslErp.Common.Domain.Interfaces;

public interface IAutoIncrementalEntity<out TKey>
    where TKey : IEquatable<TKey>, IComparable
{
    public TKey Id { get;}
}
