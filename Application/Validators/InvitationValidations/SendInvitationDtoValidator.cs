using Application.Dtos.InvitationDtos;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.InvitationValidations;

public class SendInvitationDtoValidator : AbstractValidator<SendInvitationDto>
{
    public SendInvitationDtoValidator()
    {
        RuleFor(x => x.ReceiverEmail)
            .NotEmpty().WithMessage("Receiver email is required.")
            .EmailAddress().WithMessage("Invalid email!");

        RuleFor(x => x.ReceiverRole)
            .NotEmpty().WithMessage("Receiver role is required.")
            .Must(role => role.ToLower() == Role.Admin || role.ToLower() == Role.Member)
            .WithMessage($"Receiver role must be either '{Role.Admin}' or '{Role.Member}'.");

        RuleFor(x => x.WorkspaceId).NotEmpty().WithMessage("Workspace ID is required.");
    }
}
