using System.Linq.Expressions;

namespace MtslErp.Common.Domain.Repositories;

public interface IRepositoryBase<TEntity>

{
    Task CreateAsync(TEntity entity);
    Task CreateManyAsync(ICollection<TEntity> entity);

    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition, bool enableTracking,
        CancellationToken cancellationToken = default);

    Task<ICollection<TEntity>> GetAllAsync<TSorter>(int page, int limit,
        bool updateable,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool ascendingOrder = true,
        Expression<Func<TEntity, bool>>? condition = null,
        CancellationToken cancellationToken = default)
        where TSorter : IComparable<TSorter>;

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
