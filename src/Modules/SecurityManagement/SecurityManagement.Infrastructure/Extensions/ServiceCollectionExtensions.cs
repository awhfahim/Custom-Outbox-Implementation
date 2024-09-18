using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MtslErp.Common.Application.Providers;
using MtslErp.Common.Domain.Enums;
using MtslErp.Common.Infrastructure.Providers;
using SecurityManagement.Application;
using SecurityManagement.Application.Options;
using SecurityManagement.Domain.Entities;
using SecurityManagement.Domain.Enums;
using SecurityManagement.Infrastructure.Auth;
using SecurityManagement.Infrastructure.Persistence;

namespace SecurityManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>();
        ArgumentNullException.ThrowIfNull(jwtOptions);

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[
                            SecurityManagementApplicationConstants.AccessTokenCookieKey];

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    public static async Task<IServiceCollection> SeedAdminUserAsync(this IServiceCollection services,
        IConfiguration configuration)
    {
        var seedData = configuration.GetRequiredSection(AdminUserSeedOptions.SectionName)
            .Get<AdminUserSeedOptions>();

        ArgumentNullException.ThrowIfNull(seedData);

        var dbUrl = configuration.GetRequiredSection(ConnectionStringsOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringsOptions.ErpSoftwareDb));

        var authCryptographyService = new AuthCryptographyService();
        var dateTimeProvider = new DateTimeProvider();

        var optionsBuilder = new DbContextOptionsBuilder<SecurityManagementDbContext>();
        optionsBuilder.UseOracle(dbUrl);

        await using var dbContext = new SecurityManagementDbContext(optionsBuilder.Options);

        if (await CheckBeforeSeed(dbContext) is false)
        {
            return services;
        }

        var count = await dbContext.Users.CountAsync();

        if (count != 0)
        {
            return services;
        }

        var entity = new User
        {
            FullName = seedData.FullName,
            UserName = seedData.UserName,
            MaritalStatus = seedData.MaritalStatus,
            Gender = seedData.Gender,
            DateOfBirth = seedData.DateOfBirth,
            Status = UserStatus.Active,
            PasswordHash = await authCryptographyService.HashPasswordAsync(seedData.Password),
            CreatedAtUtc = dateTimeProvider.CurrentUtcTime
        };

        await dbContext.Users.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return services;
    }

    public static async Task<IServiceCollection> SeedPermissionsAsync(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetRequiredSection(ConnectionStringsOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringsOptions.ErpSoftwareDb));

        var dateTimeProvider = new DateTimeProvider();

        var optionsBuilder = new DbContextOptionsBuilder<SecurityManagementDbContext>().UseOracle(dbUrl);

        await using var dbContext = new SecurityManagementDbContext(optionsBuilder.Options);

        if (await CheckBeforeSeed(dbContext) is false)
        {
            return services;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await AddPermissions(dbContext, dateTimeProvider);
            await AddPermissionGroups(dbContext, dateTimeProvider);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
        finally
        {
            await transaction.CommitAsync();
        }

        await SeedRelationalPermissionGroups(dbContext, dateTimeProvider);
        return services;
    }

    private static async Task SeedRelationalPermissionGroups(SecurityManagementDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        var keys = SecurityManagementApplicationConstants
            .MatchablePermissionCategoryMapper
            .Select(x => x.Key.ToString()).ToList();

        var dataSet = await dbContext.AuthorizablePermissionGroups
            .Where(e => keys.Contains(e.Label))
            .AsNoTracking()
            .ToListAsync();

        foreach (var (key, value) in SecurityManagementApplicationConstants.MatchablePermissionCategoryMapper)
        {
            var permissionLabels = value.Select(x => x.ToString()).ToList();
            var data = dataSet.FirstOrDefault(x => x.Label == key.ToString());

            if (data is null)
            {
                continue;
            }

            foreach (var permissionLabel in permissionLabels)
            {
                await dbContext.AuthorizablePermissions
                    .Where(e => e.GroupId == null && e.Label == permissionLabel)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.GroupId, data.Id)
                        .SetProperty(x => x.UpdatedAtUtc, dateTimeProvider.CurrentUtcTime));
            }
        }
    }

    private static async Task AddPermissions(SecurityManagementDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        if (EF.IsDesignTime)
        {
            return;
        }

        var permissions = Enum.GetNames<MatchablePermission>();

        var existingPermissions = await dbContext.AuthorizablePermissions
            .Where(e => permissions.Contains(e.Label))
            .Select(x => x.Label)
            .AsNoTracking()
            .ToListAsync();

        var insertablePermissions = permissions
            .Where(e => existingPermissions.Contains(e) is false)
            .Select(elem => new AuthorizablePermission
            {
                Label = elem, CreatedAtUtc = dateTimeProvider.CurrentUtcTime, GroupId = null
            })
            .ToList();

        await dbContext.AuthorizablePermissions.AddRangeAsync(insertablePermissions);
    }

    private static async Task AddPermissionGroups(SecurityManagementDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        var permissionsGroups = Enum.GetNames<MatchablePermissionGroup>();

        if (permissionsGroups.Length == 0)
        {
            return;
        }

        var existingPermissionGroups = await dbContext.AuthorizablePermissionGroups
            .Where(e => permissionsGroups.Contains(e.Label))
            .Select(x => x.Label)
            .AsNoTracking()
            .ToListAsync();

        var insertablePermissionsGroups = permissionsGroups
            .Where(e => existingPermissionGroups.Contains(e) is false)
            .Select(elem => new AuthorizablePermissionGroup
            {
                Label = elem, CreatedAtUtc = dateTimeProvider.CurrentUtcTime,
            })
            .ToList();

        await dbContext.AuthorizablePermissionGroups.AddRangeAsync(insertablePermissionsGroups);
    }

    private static async Task<bool> CheckBeforeSeed(SecurityManagementDbContext dbContext)
    {
        if (EF.IsDesignTime)
        {
            return false;
        }

        var canConnect = await dbContext.Database.CanConnectAsync();

        if (canConnect is false)
        {
            return false;
        }

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        return pendingMigrations.Any() is false;
    }
}
