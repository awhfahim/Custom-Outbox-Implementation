using FluentValidation;
using SecurityManagement.Domain;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Validators;

public class UserSignUpRequestValidator : AbstractValidator<UserSignupRequest>
{
    public UserSignUpRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(SecurityManagementDomainConstants.UserEntity.FullNameMaxLength);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(SecurityManagementDomainConstants.UserEntity.UserNameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(SecurityManagementDomainConstants.UserEntity.EmailMaxLength)
            .When(x => string.IsNullOrEmpty(x.Email) is false);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 128);

        RuleFor(x => x.Gender).NotEmpty();

        RuleFor(x => x.MaritalStatus).NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.PhoneNumber) is false);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(SecurityManagementDomainConstants.UserEntity.AddressMaxLength)
            .When(x => string.IsNullOrEmpty(x.Address) is false);

        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}
