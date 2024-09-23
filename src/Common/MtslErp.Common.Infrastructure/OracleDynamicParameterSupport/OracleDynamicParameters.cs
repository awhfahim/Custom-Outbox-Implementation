using System.Data;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace MtslErp.Common.Infrastructure.OracleDynamicParameterSupport;

public sealed class OracleDynamicParameters : SqlMapper.IDynamicParameters
{
    private readonly List<OracleParameter> _oracleParameters = [];

    public void Add(string name, OracleDbType oracleDbType, object? value = null,
        ParameterDirection direction = ParameterDirection.Input, bool isNullable = true)
    {
        var parameter = new OracleParameter
        {
            ParameterName = name,
            Value = value,
            OracleDbType = oracleDbType,
            Direction = direction,
            IsNullable = isNullable
        };
        _oracleParameters.Add(parameter);
    }

    public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
    {
        var oracleCommand = (OracleCommand)command;
        oracleCommand.Parameters.AddRange(_oracleParameters.ToArray());
    }
}
