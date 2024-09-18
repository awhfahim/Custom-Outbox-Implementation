using FluentValidation;
using SecurityManagement.Domain;

namespace SecurityManagement.Application.Features.AuthFeatures.Validators;

public class PermissionCreateOrUpdateModelValidator : AbstractValidator<PermissionCreateOrUpdateModel>
{
    public PermissionCreateOrUpdateModelValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(SecurityManagementDomainConstants.AuthorizablePermissionEntity.LabelMaxLength);
    }
}
