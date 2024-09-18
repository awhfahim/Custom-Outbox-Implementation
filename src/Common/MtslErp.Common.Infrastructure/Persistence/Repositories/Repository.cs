using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Interfaces;
using MtslErp.Common.Domain.Repositories;

namespace MtslErp.Common.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TKey>
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TEntity> EntityDbSet;

    protected Repository(DbContext context)
    {
        DbContext = context;
        EntityDbSet = DbContext.Set<TEntity>();
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        await EntityDbSet.AddAsync(entity).ConfigureAwait(false);
    }

    public async Task CreateManyAsync(ICollection<TEntity> entity)
    {
        await EntityDbSet.AddRangeAsync(entity).ConfigureAwait(false);
    }

    public virtual Task RemoveAsync(TEntity entityToDelete)
    {
        return Task.Run(() =>
        {
            if (DbContext.Entry(entityToDelete).State is EntityState.Detached)
            {
                EntityDbSet.Attach(entityToDelete);
            }

            EntityDbSet.Remove(entityToDelete);
        });
    }

    public Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition,
        CancellationToken cancellationToken = default)
    {
        return EntityDbSet.Where(condition).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> condition, bool updateable,
        CancellationToken cancellationToken = default)
    {
        var query = EntityDbSet.Where(condition);
        if (updateable is false)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition,
        CancellationToken cancellationToken = default)
    {
        return EntityDbSet.AnyAsync(condition, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entityToUpdate)
    {
        return Task.Run(() =>
        {
            EntityDbSet.Attach(entityToUpdate);
            DbContext.Entry(entityToUpdate).State = EntityState.Modified;
        });
    }

    public virtual Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition = null,
        CancellationToken ct = default)
    {
        return condition is not null
            ? EntityDbSet.CountAsync(condition, ct)
            : EntityDbSet.CountAsync(ct);
    }

    public Task TrackEntityAsync<T>(T entity) where T : class
    {
        return Task.Run(() => DbContext.Set<T>().Attach(entity));
    }

    public Task TrackEntityAsync(TEntity entity)
    {
        return Task.Run(() => DbContext.Set<TEntity>().Attach(entity));
    }

    public Task ModifyEntityStateToAddedAsync(TEntity entity)
    {
        return Task.Run(() =>
        {
            if (DbContext.Entry(entity).State is not EntityState.Added)
            {
                DbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }

    public Task ModifyEntityStateToAddedAsync<T>(T entity)
    {
        return Task.Run(() =>
        {
            if (entity is null)
            {
                return;
            }

            if (DbContext.Entry(entity).State is not EntityState.Added)
            {
                DbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }
}
