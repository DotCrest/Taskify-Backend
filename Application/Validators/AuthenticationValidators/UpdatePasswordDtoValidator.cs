<<<<<<< HEAD
﻿using Application.Dtos.AuthenticationDtos;
=======
﻿using Application.Dtos;
>>>>>>> develop
using FluentValidation;

namespace Application.Validators.AuthenticationValidators;

public sealed class UpdatePasswordDtoValidator : AbstractValidator<UpdatePasswordDto>
{
    public UpdatePasswordDtoValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
        RuleFor(u => u.NewPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^\da-zA-Z]").WithMessage("Password must contain at least one special character");
        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required")
            .Equal(u => u.NewPassword).WithMessage("Confirm Password must match New Password");
    }
}
