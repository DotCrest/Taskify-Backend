using Application.Dtos.SpaceDtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators.SpaceValidators;

public class CreateSpaceDtoValidator : AbstractValidator<CreateSpaceDto>
{
    private static readonly Regex HexColorRegex = new Regex(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);
    public CreateSpaceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.IconColor)
            .Matches(HexColorRegex).WithMessage("{PropertyName} must be a valid hex color code.")
            .When(x => !string.IsNullOrEmpty(x.IconColor));

        RuleFor(x => x.WorkspaceId)
            .NotNull().WithMessage("WorkspaceId is required.")
            .GreaterThan(0).WithMessage("WorkspaceId must be a positive integer.");
    }
}
