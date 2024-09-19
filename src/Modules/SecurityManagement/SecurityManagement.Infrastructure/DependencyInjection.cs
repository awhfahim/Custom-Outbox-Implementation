using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Services;
using MtslErp.Common.Infrastructure.Caching;
using SecurityManagement.Application;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Application.Providers;
using SecurityManagement.Domain.Repositories;
using SecurityManagement.Infrastructure.Auth;
using SecurityManagement.Infrastructure.Persistence;
using SecurityManagement.Infrastructure.Persistence.Repositories;
using SecurityManagement.Infrastructure.Providers;

namespace SecurityManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSecurityManagementInfrastructureServices(
        this IServiceCollection services)
    {
        services.TryAddScoped<IKeyValueCache, RedisKeyValueCache>();
        services.TryAddSingleton<IJwtProvider, JwtProvider>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.TryAddSingleton<IAuthCryptographyService, AuthCryptographyService>();

        services.TryAddScoped<ISecurityManagementAppUnitOfWork, SecurityManagementAppUnitOfWork>();
        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IAuthorizableRoleRepository, AuthorizableRoleRepository>();
        services.TryAddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.TryAddScoped<IUserRoleRepository, UserRoleRepository>();
        services.TryAddScoped<IAuthorizablePermissionRepository, AuthorizablePermissionRepository>();
        services.TryAddScoped<IAuthorizablePermissionGroupRepository,
            AuthorizablePermissionGroupRepository>();

        return services;
    }
}
