using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintFactoryManagement.Application.Options;
using PrintFactoryManagement.Infrastructure.Persistence;

namespace PrintFactoryManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetRequiredSection(ConnectionStringOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringOptions.ConnectionString));

        ArgumentNullException.ThrowIfNull(dbUrl);

        var optionsBuilder = new DbContextOptionsBuilder<PfmDbContext>();
        optionsBuilder.UseOracle(dbUrl);

        using (var dbContext = new PfmDbContext(optionsBuilder.Options))
        {
            Console.WriteLine(dbContext.Database.CanConnect());
        }

        services.AddDbContext<PfmDbContext>(
            (sp, dbContextOptions) => dbContextOptions
                .UseOracle(dbUrl, x =>
                {
                    x.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
                })
        );

        return services;
    }
}
