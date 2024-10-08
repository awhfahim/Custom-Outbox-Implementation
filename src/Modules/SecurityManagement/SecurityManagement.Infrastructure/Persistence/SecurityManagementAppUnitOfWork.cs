using SecurityManagement.Application;
using SecurityManagement.Domain.Repositories;
using SecurityManagement.Domain.Repositories.Outbox;

namespace SecurityManagement.Infrastructure.Persistence;

public class SecurityManagementAppUnitOfWork : SecurityManagementUnitOfWork, ISecurityManagementAppUnitOfWork
{
    public SecurityManagementAppUnitOfWork(SecurityManagementDbContext dbContext,
        IAuthorizablePermissionGroupRepository authorizablePermissionGroupRepository,
        IAuthorizablePermissionRepository authorizablePermissionRepository,
        IAuthorizableRoleRepository authorizableRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        ISecurityManagementOutboxRepository securityManagementOutboxRepository) : base(dbContext)
    {
        AuthorizablePermissionGroupRepository = authorizablePermissionGroupRepository;
        AuthorizablePermissionRepository = authorizablePermissionRepository;
        AuthorizableRoleRepository = authorizableRoleRepository;
        RolePermissionRepository = rolePermissionRepository;
        UserRoleRepository = userRoleRepository;
        UserRepository = userRepository;
        SecurityManagementOutboxRepository = securityManagementOutboxRepository;
    }

    public IAuthorizablePermissionGroupRepository AuthorizablePermissionGroupRepository { get; }
    public IAuthorizablePermissionRepository AuthorizablePermissionRepository { get; }
    public IAuthorizableRoleRepository AuthorizableRoleRepository { get; }
    public IRolePermissionRepository RolePermissionRepository { get; }
    public IUserRoleRepository UserRoleRepository { get; }
    public IUserRepository UserRepository { get; }
    public ISecurityManagementOutboxRepository SecurityManagementOutboxRepository { get; }
}
