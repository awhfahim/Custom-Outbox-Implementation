using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Repositories;

public interface ITimestampedEntityRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, ITimestamp

{
    Task<PagedData<TEntity>> GetPagedDataForDynamicQueryAsync(DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider, CancellationToken cancellationToken = default);
}
