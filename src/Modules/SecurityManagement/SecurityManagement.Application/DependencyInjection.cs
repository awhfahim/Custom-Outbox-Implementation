using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SecurityManagement.Application.Features.AuthFeatures;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSecurityManagementApplicationServices(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.TryAddScoped<IAuthPermissionService, AuthPermissionService>();
        services.TryAddScoped<IAuthRoleService, AuthRoleService>();
        services.TryAddScoped<IUserRoleService, UserRoleService>();
        services.TryAddScoped<IUserService, UserService>();

        return services;
    }
}
