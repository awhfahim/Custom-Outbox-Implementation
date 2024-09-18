using System.Data.Common;

namespace MtslErp.Common.Domain.Interfaces;
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    void Save();
    Task SaveAsync();
    Task<DbTransaction> BeginTransactionAsync();
}
