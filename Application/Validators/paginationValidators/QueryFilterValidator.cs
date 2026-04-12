using Application.Shared.Pagination;
using FluentValidation;

namespace Application.Validators.paginationValidators;

public class QueryFilterValidator : AbstractValidator<QueryFilter>
{
    public QueryFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");
    }
}
