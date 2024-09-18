using FluentValidation;
using SecurityManagement.Domain;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Validators;

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(1, SecurityManagementDomainConstants.UserEntity.UserNameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 100);

        RuleFor(x => x.RememberMe).NotNull();
    }
}
