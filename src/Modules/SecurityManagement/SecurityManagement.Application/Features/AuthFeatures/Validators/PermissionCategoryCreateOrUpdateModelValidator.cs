using FluentValidation;
using SecurityManagement.Domain;

namespace SecurityManagement.Application.Features.AuthFeatures.Validators;

public class PermissionGroupCreateOrUpdateModelValidator
    : AbstractValidator<PermissionGroupCreateOrUpdateModel>
{
    public PermissionGroupCreateOrUpdateModelValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(SecurityManagementDomainConstants.AuthorizablePermissionGroupEntity.LabelMaxLength);
    }
}
