using System.ComponentModel.DataAnnotations;

namespace MtslErp.Common.Infrastructure.Data;

public sealed class PrintFactoryManagementDb
{
    [Required] public required string ConnectionString { get; init; }
}
