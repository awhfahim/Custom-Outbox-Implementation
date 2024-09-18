using System.Linq.Expressions;
using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Repositories;

public interface IRepositoryBase<TEntity, in TKey>
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable
{
    Task CreateAsync(TEntity entity);
    Task CreateManyAsync(ICollection<TEntity> entity);

    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition,
        CancellationToken cancellationToken = default);
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition, bool updateable,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entityToUpdate);
    Task RemoveAsync(TEntity entityToDelete);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition = null,
        CancellationToken ct = default);

    Task ModifyEntityStateToAddedAsync<T>(T entity);
    Task ModifyEntityStateToAddedAsync(TEntity entity);
    Task TrackEntityAsync<T>(T entity) where T : class;
    Task TrackEntityAsync(TEntity entity);
}
