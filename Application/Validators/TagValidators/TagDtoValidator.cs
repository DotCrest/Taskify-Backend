using Application.Dtos.TagDtos;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators.TagValidators;


public class TagDtoValidator : AbstractValidator<TagDto>
{
    private static readonly Regex HexColorRegex = new Regex(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);
    public TagDtoValidator()
    {
        RuleFor(x => x.Color)
            .Matches(HexColorRegex).WithMessage("{PropertyName} must be a valid hex color code.")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.WorkspaceId)
            .NotNull().WithMessage("WorkspaceId is required.")
            .GreaterThan(0).WithMessage("WorkspaceId must be a positive integer.");
    }
}
