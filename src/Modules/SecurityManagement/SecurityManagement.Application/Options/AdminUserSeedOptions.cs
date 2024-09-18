using SecurityManagement.Domain.Enums;

namespace SecurityManagement.Application.Options;

public class AdminUserSeedOptions
{
    public const string SectionName = "AdminUserSeedOptions";
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string Password { get; set; }
    public required MaritalStatus MaritalStatus { get; set; }
    public required Gender Gender { get; set; }
    public required DateOnly DateOfBirth { get; set; }
}
