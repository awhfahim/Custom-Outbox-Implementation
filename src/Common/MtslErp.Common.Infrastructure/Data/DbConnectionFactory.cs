using System.Data.Common;
using Microsoft.Extensions.Options;
using MtslErp.Common.Application.Data;
using MtslErp.Common.Application.Options;
using Oracle.ManagedDataAccess.Client;

namespace MtslErp.Common.Infrastructure.Data;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IOptions<ConnectionStringOptions> options)
    {
        _connectionString = options.Value.OracleDbConnectionString;
    }

    public async Task<DbConnection?> OpenConnectionAsync()
    {
        try
        {
            var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
