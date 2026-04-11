using Application.Dtos.InvitationDtos;
using FluentValidation;

namespace Application.Validators.InvitationValidations;

public class GetInvitationDtoValidator : AbstractValidator<GetInvitationDto>
{
    public GetInvitationDtoValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId is required.");
        RuleFor(x => x.Status)
            .Must(status => new[] { "pending", "accepted", "expired" }.Contains(status.ToLower()))
            .WithMessage("Status must be either 'pending', 'accepted', or 'expired'.")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }

}
