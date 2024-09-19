using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityManagement.Application;
using SecurityManagement.Application.Options;
using SecurityManagement.Infrastructure;
using SecurityManagement.Infrastructure.Extensions;
using MassTransit;
using MtslErp.Common.Application.Options;

namespace SecurityManagement.HttpApi;

public static class SecurityManagementExtensions
{
    public static async Task<IServiceCollection> RegisterSecurityManagementAsync(
        this IServiceCollection services, IConfiguration configuration, string? prefix = null)
    {
        services.AddAntiforgery(options =>
        {
            options.Cookie.Name = SecurityManagementApplicationConstants.XsrfTokenCookieKey;
            options.HeaderName = SecurityManagementApplicationConstants.XsrfTokenHeaderKey;
        });

        var recaptchaOptions = configuration.GetRequiredSection(GoogleRecaptchaOptions.SectionName)
            .Get<GoogleRecaptchaOptions>();

        ArgumentNullException.ThrowIfNull(recaptchaOptions);

        services.AddHttpClient(GoogleRecaptchaOptions.SectionName,
            httpClient => httpClient.BaseAddress = new Uri(recaptchaOptions.VerificationEndpoint));

        await services.SeedAdminUserAsync(configuration);
        await services.SeedPermissionsAsync(configuration);
        services.AddJwtAuth(configuration);
        services.AddDatabaseConfig(configuration);
        services.RegisterSecurityManagementApplicationServices();
        services.RegisterSecurityManagementInfrastructureServices();
        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {

    }
}

public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigureSecurityManagementEndpoints(this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext context, RabbitMqSettings rabbitMqSettings)
    {

    }
}
