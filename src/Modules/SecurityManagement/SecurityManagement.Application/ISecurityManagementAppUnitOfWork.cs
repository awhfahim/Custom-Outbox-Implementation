using MtslErp.Common.Domain.Interfaces;
using SecurityManagement.Domain.Repositories;

namespace SecurityManagement.Application;

public interface ISecurityManagementAppUnitOfWork : IUnitOfWork
{
    public IAuthorizablePermissionGroupRepository AuthorizablePermissionGroupRepository { get; }
    public IAuthorizablePermissionRepository AuthorizablePermissionRepository { get; }
    public IAuthorizableRoleRepository AuthorizableRoleRepository { get; }
    public IRolePermissionRepository RolePermissionRepository { get; }
    public IUserRoleRepository UserRoleRepository { get; }
    public IUserRepository UserRepository { get; }
}
