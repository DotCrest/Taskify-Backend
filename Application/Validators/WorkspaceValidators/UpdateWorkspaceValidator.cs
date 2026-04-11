using Application.Dtos.WorkspaceDtos;
using FluentValidation;

namespace Application.Validators.WorkspaceValidators;

public class UpdateWorkspaceValidator : AbstractValidator<UpdateWorkspaceDto>
{
    public UpdateWorkspaceValidator()
    {
        RuleFor(w => w.Name).NotEmpty()
         .WithMessage("Name is required")
         .MinimumLength(4).WithMessage("Name must be at least 4 characters")
         .MaximumLength(50).WithMessage("Name can't exceed 50 characters");
    }
}
