using System.ComponentModel.DataAnnotations;

namespace PrintFactoryManagement.Infrastructure.Data;

public sealed class PrintFactoryManagementDb
{
    [Required] public required string ConnectionString { get; init; }
}
