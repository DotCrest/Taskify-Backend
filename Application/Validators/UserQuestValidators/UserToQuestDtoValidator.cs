using Application.Dtos.UserQuestDtos;
using FluentValidation;

namespace Application.Validators.UserQuestValidators;

public class UserToQuestDtoValidator : AbstractValidator<UserToQuestDto>
{
    public UserToQuestDtoValidator()
    {
        RuleFor(u => u.AssigneeId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(u => u.QuestId)
            .GreaterThan(0).WithMessage("QuestId must be greater than 0.");

        RuleFor(u => u.SpaceId)
            .GreaterThan(0).WithMessage("SpaceId must be greater than 0.");

        RuleFor(u => u.WorkspaceId)
            .GreaterThan(0).WithMessage("WorkspaceId must be greater than 0.");
    }
}
