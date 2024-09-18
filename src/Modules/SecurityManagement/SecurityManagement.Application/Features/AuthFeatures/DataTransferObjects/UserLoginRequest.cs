namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record UserLoginRequest(string UserName, string Password, bool RememberMe);
