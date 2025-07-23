using FluentValidation;
using PolicyManagement.Models.DTOs;

namespace PolicyManagement.Validators;

public class CreatePolicyDtoValidator : AbstractValidator<CreatePolicyDto>
{
    public CreatePolicyDtoValidator()
    {
        RuleFor(p => p.PolicyId)
            .NotEmpty().WithMessage("Policy ID is required");

        RuleFor(p => p.PolicyName)
            .NotEmpty().WithMessage("Policy name is required");

        RuleFor(p => p.Premium)
            .GreaterThan(0).WithMessage("Premium must be greater than 0");

        RuleFor(p => p.Coverage)
            .NotEmpty().WithMessage("Coverage is required");

        RuleFor(p => p.ValidityInYears)
            .GreaterThan(0).WithMessage("Validity must be at least 1 year");
    }
}