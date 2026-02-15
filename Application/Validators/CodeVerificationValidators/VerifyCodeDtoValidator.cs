using Application.Dtos;
using FluentValidation;

namespace Application.Validators.CodeVerificationValidators;

public class VerifyCodeDtoValidator : AbstractValidator<VerifyCodeDto>
{
    public VerifyCodeDtoValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .EmailAddress().WithMessage("{PropertyName} must be a valid email address");
    }
}
