using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Infrastructure.Extensions;
using MtslErp.Common.Infrastructure.Persistence.Repositories;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SecurityManagement.Domain.Repositories;

namespace SecurityManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public UserRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }

    public async Task<PagedData<UserResponse>> GetPagedDataForDynamicQueryAsync(DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider, CancellationToken cancellationToken = default)
    {
        var initialQuery = EntityDbSet.WhereDynamic(dto.Filters, reflectionCacheProvider);

        initialQuery = dto.Sorters.Count == 0
            ? initialQuery.OrderByDescending(x => x.CreatedAtUtc)
            : initialQuery.OrderBy(dto.Sorters, reflectionCacheProvider);

        var count = await initialQuery.CountAsync(cancellationToken);

        var res = initialQuery
            .PaginateQueryable(dto.Page, dto.Size)
            .AsNoTracking()
            .Select(x =>
                new UserResponse
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    MaritalStatus = x.MaritalStatus,
                    Gender = x.Gender,
                    ProfilePictureUri = x.ProfilePictureUri,
                    DateOfBirth = x.DateOfBirth,
                    Address = x.Address,
                    Status = x.Status,
                    CreatedAtUtc = x.CreatedAtUtc,
                    UpdatedAtUtc = x.UpdatedAtUtc,
                    IsArchived = x.IsArchived,
                    ArchivedAtUtc = x.ArchivedAtUtc
                });

        return new PagedData<UserResponse>(await res.ToListAsync(cancellationToken), count);
    }
}
