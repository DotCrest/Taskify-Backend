using Application.Dtos.QuestDtos;
using FluentValidation;

namespace Application.Validators.QuestValidators
{
    public class UpdateQuestValidator : AbstractValidator<QuestToUpdateDto>
    {
        public UpdateQuestValidator()
        {
            RuleFor(q => q.Title)
              .NotEmpty().WithMessage("Title is required.")
              .MinimumLength(4).WithMessage("Title greater than 3")
              .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
            RuleFor(q => q.Priority)
                .InclusiveBetween(0, 4).WithMessage("Priority must be between 0 and 4.");
            RuleFor(q => q.Status)
               .InclusiveBetween(0, 4).WithMessage("Status must be between 0 and 4.");
            RuleFor(q => q.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        }
    }
}
