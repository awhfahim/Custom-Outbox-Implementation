using MtslErp.Common.Infrastructure.Persistence.Repositories;
using SecurityManagement.Domain.Entities;
using SecurityManagement.Domain.Repositories;

namespace SecurityManagement.Infrastructure.Persistence.Repositories;

public class AuthorizablePermissionGroupRepository : Repository<AuthorizablePermissionGroup>,
    IAuthorizablePermissionGroupRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public AuthorizablePermissionGroupRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }
}
