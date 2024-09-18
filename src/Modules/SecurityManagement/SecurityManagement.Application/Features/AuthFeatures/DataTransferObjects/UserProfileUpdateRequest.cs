namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record UserProfileUpdateRequest(string FullName, string? Email, string? PhoneNumber);
