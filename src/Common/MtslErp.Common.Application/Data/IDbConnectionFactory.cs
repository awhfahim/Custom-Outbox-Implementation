using System.Data.Common;

namespace MtslErp.Common.Application.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection?> OpenConnectionAsync();
}
