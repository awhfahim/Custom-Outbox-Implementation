namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record UserSignUpRequest(string FullName, string UserName, string Password,  string ConfirmPassword);
