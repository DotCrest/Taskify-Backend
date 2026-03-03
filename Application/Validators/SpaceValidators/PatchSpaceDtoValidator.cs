using Application.Dtos.SpaceDtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators.SpaceValidators;

public class PatchSpaceDtoValidator : AbstractValidator<PatchSpaceDto>
{
    private static readonly Regex HexColorRegex = new Regex(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);
    public PatchSpaceDtoValidator()
    {
        RuleFor(x => x.Name)
           .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
           .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.IconColor)
            .Matches(HexColorRegex).WithMessage("{PropertyName} must be a valid hex color code.")
            .When(x => !string.IsNullOrEmpty(x.IconColor));

    }
}
