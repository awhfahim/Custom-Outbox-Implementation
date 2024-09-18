using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.Repositories;
using MtslErp.Common.Infrastructure.Extensions;

namespace MtslErp.Common.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class
{
    protected readonly DbContext DatabaseContext;
    protected readonly DbSet<TEntity> EntityDbSet;

    protected Repository(DbContext context)
    {
        DatabaseContext = context;
        EntityDbSet = DatabaseContext.Set<TEntity>();
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
            if (DatabaseContext.Entry(entityToDelete).State is EntityState.Detached)
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
            DatabaseContext.Entry(entityToUpdate).State = EntityState.Modified;
        });
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync<TSorter>(int page, int limit,
        bool updateable,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool ascendingOrder = true,
        Expression<Func<TEntity, bool>>? condition = null,
        CancellationToken cancellationToken = default)
        where TSorter : IComparable<TSorter>
    {
        var query = EntityDbSet.AsQueryable();
        if (condition is not null)
        {
            query = EntityDbSet.Where(condition);
        }

        query = ascendingOrder
            ? query.OrderBy(orderBy)
            : query.OrderByDescending(orderBy);

        if (updateable is false)
        {
            query = query.AsNoTracking();
        }

        return await query.PaginateQueryable(page, limit).ToListAsync(cancellationToken);
    }

    public virtual Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition = null,
        CancellationToken cancellationToken = default)
    {
        return condition is not null
            ? EntityDbSet.CountAsync(condition, cancellationToken)
            : EntityDbSet.CountAsync(cancellationToken);
    }

    public Task TrackEntityAsync<T>(T entity) where T : class
    {
        return Task.Run(() => DatabaseContext.Set<T>().Attach(entity));
    }

    public Task TrackEntityAsync(TEntity entity)
    {
        return Task.Run(() => DatabaseContext.Set<TEntity>().Attach(entity));
    }

    public Task ModifyEntityStateToAddedAsync(TEntity entity)
    {
        return Task.Run(() =>
        {
            if (DatabaseContext.Entry(entity).State is not EntityState.Added)
            {
                DatabaseContext.Entry(entity).State = EntityState.Added;
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

            if (DatabaseContext.Entry(entity).State is not EntityState.Added)
            {
                DatabaseContext.Entry(entity).State = EntityState.Added;
            }
        });
    }

    private static (int page, int limit) AvoidNegativeOrZeroPagination(int page, int limit)
    {
        var pagination = (page, limit);
        if (page <= 0)
        {
            pagination.page = 1;
        }

        if (limit <= 0)
        {
            pagination.limit = 1;
        }

        return pagination;
    }
}
