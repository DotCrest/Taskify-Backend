using Application.Shared.Pagination;
using FluentValidation;

namespace Application.Validators.QuestValidators;

public class QuestCustomQueryFilterValidator : AbstractValidator<QuestCustomQueryFilter>
{
    public QuestCustomQueryFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");

        RuleFor(x => x.TagId)
            .GreaterThan(0)
            .When(x => x.TagId != null)
            .WithMessage("TagId must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId != null)
            .WithMessage("CategoryId must be greater than 0.");

        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || new[] { "todo", "inprogress", "complete" }.Contains(s.ToLower()))
            .WithMessage("Status must be either 'todo', 'inprogress', 'complete'");

        RuleFor(x => x.Priority)
            .Must(p => string.IsNullOrEmpty(p) || new[] { "low", "normal", "high", "urgent" }.Contains(p.ToLower()))
            .WithMessage("Priority must be either 'low', 'normal', 'high', 'urgent'");
    }
}
