using SecurityManagement.Domain.Enums;

namespace SecurityManagement.Domain.DataTransferObjects.Response;

public record UserResponse
{
    public long Id { get; init; }
    public required string UserName { get; init; }
    public required string FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public required MaritalStatus MaritalStatus { get; init; }
    public required Gender Gender { get; init; }
    public string? ProfilePictureUri { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public string? Address { get; init; }
    public required UserStatus Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public bool IsArchived { get; init; }
    public DateTime? ArchivedAtUtc { get; init; }
}
