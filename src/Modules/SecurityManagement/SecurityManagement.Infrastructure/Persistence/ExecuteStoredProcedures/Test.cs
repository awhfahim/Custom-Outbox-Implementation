using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Data;

namespace SecurityManagement.Infrastructure.Persistence.ExecuteStoredProcedures;

public class Test(IServiceProvider serviceProvider)
{
    public async Task<string?> TestMethod()
    {
        var connectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
        await using var connection = await connectionFactory.OpenConnectionAsync();

        if (connection is null)
        {
            return null;
        }

        var parameters = new DynamicParameters();
        parameters.Add("result", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

        await connection.ExecuteAsync("ACM.PrintHelloWorld", parameters,
            commandType: CommandType.StoredProcedure);

        // Retrieve the output value
        return parameters.Get<string>("result");
    }
}
