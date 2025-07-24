using FluentValidation;
using PolicyManagement.DTOs;

namespace PolicyManagement.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Roles are required")
            .Must(r => r.Count > 0).WithMessage("At least one role is required");
            
    }
}