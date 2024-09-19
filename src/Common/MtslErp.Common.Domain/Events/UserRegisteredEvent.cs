using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Events;

public sealed class UserRegisteredEvent
{
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUri { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public string? Address { get; set; }
}
