using System.ComponentModel.DataAnnotations;

namespace PrintFactoryManagement.Application.Options;

public record ConnectionStringOptions
{
    public const string SectionName = "PrintFactoryManagementDb";
    [Required] public required string ConnectionString { get; init; }
}
