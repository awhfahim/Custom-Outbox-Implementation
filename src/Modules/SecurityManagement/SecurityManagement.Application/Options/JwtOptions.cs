using System.ComponentModel.DataAnnotations;

namespace SecurityManagement.Application.Options;

public record JwtOptions
{
    public const string SectionName = "JwtOptions";
    [Required] public required string Secret { get; init; }
    [Required] public required uint AccessTokenExpiryMinutes { get; init; }
    [Required] public required uint RefreshTokenExpiryMinutes { get; init; }
}
