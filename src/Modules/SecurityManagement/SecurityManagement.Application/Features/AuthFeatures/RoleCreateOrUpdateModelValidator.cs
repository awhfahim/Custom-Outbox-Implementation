using FluentValidation;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

namespace SecurityManagement.Application.Features.AuthFeatures;

public class RoleCreateOrUpdateModelValidator : AbstractValidator<RoleCreateOrUpdateModel>
{
    public RoleCreateOrUpdateModelValidator()
    {
        RuleFor(x => x.Label).NotEmpty();
    }
}
