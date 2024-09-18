using System.ComponentModel.DataAnnotations;

namespace SecurityManagement.Application.Options;

public record ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";
    [Required] public required string ErpSoftwareDb { get; init; }
    [Required] public required string StackExchangeRedisUrl { get; init; }
}
