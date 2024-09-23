using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Options;
using PrintFactoryManagement.Infrastructure.Persistence;

namespace PrintFactoryManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetRequiredSection(ConnectionStringOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringOptions.OracleDbConnectionString));

        ArgumentNullException.ThrowIfNull(dbUrl);

        var optionsBuilder = new DbContextOptionsBuilder<PrintFactoryDbContext>();
        optionsBuilder.UseOracle(dbUrl);

        using (var dbContext = new PrintFactoryDbContext(optionsBuilder.Options))
        {
            Console.WriteLine(dbContext.Database.CanConnect());
        }

        services.AddDbContext<PrintFactoryDbContext>(
            (dbContextOptions) => dbContextOptions
                .UseOracle(dbUrl,
                    x => { x.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19); })
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );

        return services;
    }
}
