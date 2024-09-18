using MtslErp.Common.Domain.Interfaces;
using SecurityManagement.Domain.Enums;

namespace SecurityManagement.Domain.Entities;

public sealed class User : IAutoIncrementalEntity<long>, ITimestamp, IArchivable
{
    public long Id { get;  }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public required MaritalStatus MaritalStatus { get; set; }
    public required Gender Gender { get; set; }
    public string? ProfilePictureUri { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
    public required UserStatus Status { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAtUtc { get; set; }
}
