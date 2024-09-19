using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Infrastructure.Persistence;

public abstract class UnitOfWorkBase : IUnitOfWork
{
    private readonly DbContext _dbContext;

    protected UnitOfWorkBase(DbContext dbContext) => _dbContext = dbContext;

    public virtual void Dispose() => _dbContext.Dispose();

    public virtual ValueTask DisposeAsync() => _dbContext.DisposeAsync();

    public virtual void Save() => _dbContext.SaveChanges();

    public virtual async Task SaveAsync() => await _dbContext.SaveChangesAsync();

    public async Task<DbTransaction> BeginTransactionAsync()
    {
        var trx = await _dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
        return trx.GetDbTransaction();
    }
}
