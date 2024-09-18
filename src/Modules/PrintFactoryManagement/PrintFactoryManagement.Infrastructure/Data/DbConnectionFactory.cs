using System.Data.Common;
using Microsoft.Extensions.Options;
using MtslErp.Common.Application.Data;
using MtslErp.Common.Infrastructure.Data;
using Oracle.ManagedDataAccess.Client;

namespace PrintFactoryManagement.Infrastructure.Data;

internal sealed class PfmDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PfmDbConnectionFactory(IOptions<PrintFactoryManagementDb> options)
    {
        _connectionString = options.Value.ConnectionString;
    }
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        var connection = new OracleConnection(_connectionString);

        await connection.OpenAsync();

        return connection;
    }
}
