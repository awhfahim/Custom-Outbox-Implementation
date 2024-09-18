using System.ComponentModel.DataAnnotations;

namespace MtslErp.Common.Application.Options;

public record AppOptions
{
    public const string SectionName = "AppOptions";
    [Required] public required string[] AllowedOriginsForCors { get; init; }
}
