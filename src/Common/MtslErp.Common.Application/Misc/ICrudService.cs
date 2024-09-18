using SharpOutcome;

namespace MtslErp.Common.Application.Misc;

public interface ICrudService<TEntity, in TKey, in TCreateOrUpdateDto, TFailedOutcome>
    where TEntity : notnull
    where TFailedOutcome : notnull
{
    Task<ValueOutcome<TEntity, TFailedOutcome>> CreateAsync(TCreateOrUpdateDto dto);
    Task<TEntity?> ReadAsync(TKey id, CancellationToken ct = default);
    Task<ValueOutcome<TEntity, TFailedOutcome>> UpdateAsync(TKey id, TCreateOrUpdateDto dto);
    Task<bool> DeleteAsync(TKey id);
}
