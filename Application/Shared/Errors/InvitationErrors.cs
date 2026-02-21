
using Application.Common.Errors;

namespace Application.Shared.Errors;

public class InvitationErrors
{
    public static readonly Error AlreadySent =
        new Error("Invitation.AlreadySent", "Invitation already sent to this email and it's still pending");
    public static readonly Error NotFound =
        new Error("Invitation.NotFound", "Invitation is not found");
    public static readonly Error Expired =
        new Error("Invitation.Expired", "Invitation has been expired");
    public static readonly Error InvalidToken =
        new Error("Invitation.Token", "Invitation token is invalid");
    public static readonly Error AlreadyAccepted =
        new Error("Invitation.AlreadyAccepted", "Invitation has been already accepted");
}
