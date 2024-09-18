namespace MtslErp.Common.Domain.Interfaces;

public interface IEntity<TKey> where TKey : IEquatable<TKey>, IComparable
{
    public TKey Id { get; init; }
}
