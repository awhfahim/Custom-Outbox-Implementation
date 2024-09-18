using SecurityManagement.Domain.Enums;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record UserSignupRequest(
    string FullName,
    string UserName,
    string Password,
    DateOnly DateOfBirth,
    Gender Gender,
    MaritalStatus MaritalStatus,
    string? Email,
    string? Address,
    string? PhoneNumber);
