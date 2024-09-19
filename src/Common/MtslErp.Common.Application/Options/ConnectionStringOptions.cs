using System.ComponentModel.DataAnnotations;

namespace MtslErp.Common.Application.Options;

public record ConnectionStringOptions
{
    public const string SectionName = "ConnectionStringOptions";
    [Required] public required string OracleDbConnectionString { get; init; }
    [Required] public required string StackExchangeRedisUrl { get; init; }
}
