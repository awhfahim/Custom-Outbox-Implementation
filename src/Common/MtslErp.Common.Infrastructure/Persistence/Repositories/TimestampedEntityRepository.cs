using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Domain.Interfaces;
using MtslErp.Common.Domain.Repositories;
using MtslErp.Common.Infrastructure.Extensions;

namespace MtslErp.Common.Infrastructure.Persistence.Repositories;

public abstract class TimestampedEntityRepository<TEntity> : Repository<TEntity>,
    ITimestampedEntityRepository<TEntity> where TEntity : class, ITimestamp

{
    protected TimestampedEntityRepository(DbContext context) : base(context)
    {
    }

    public virtual async Task<PagedData<TEntity>> GetPagedDataForDynamicQueryAsync(DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default)
    {
        var data = EntityDbSet.WhereDynamic(dto.Filters, reflectionCacheProvider);
        data = dto.Sorters.Count == 0
            ? data.OrderByDescending(x => x.CreatedAtUtc)
            : data.OrderBy(dto.Sorters, reflectionCacheProvider);

        var count = await data.LongCountAsync(cancellationToken);
        data = data.PaginateQueryable(dto.Page, dto.Size);
        return new PagedData<TEntity>(await data.ToListAsync(cancellationToken), count);
    }
}
