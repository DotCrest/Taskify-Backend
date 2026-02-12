using Application.Dtos.AuthenticationDtos;
using FluentValidation;

namespace Application.Validators.AuthenticationValidators;

public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
