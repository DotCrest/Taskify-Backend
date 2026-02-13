using Application.Dtos.AuthenticationDtos;
using FluentValidation;

namespace Application.Validators.AuthenticationValidators;

public sealed class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(u => u.OldPassword)
            .NotEmpty().WithMessage("Old password is required");
        RuleFor(u => u.NewPassword).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^\da-zA-Z]").WithMessage("Password must contain at least one special character");
        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(u => u.NewPassword).WithMessage("Passwords do not match");
    }
}
