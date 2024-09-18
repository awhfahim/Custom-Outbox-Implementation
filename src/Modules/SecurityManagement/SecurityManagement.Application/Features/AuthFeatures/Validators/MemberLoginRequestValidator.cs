using FluentValidation;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Domain;

namespace SecurityManagement.Application.Features.AuthFeatures.Validators;

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(1, SecurityManagementDomainConstants.UserEntity.UserNameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 128);

        RuleFor(x => x.RememberMe).NotNull();
    }
}
