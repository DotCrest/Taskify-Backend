using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class VerificationCodeErrors
{
    public static readonly Error InvalidCode =
            new Error("Verification.InvalidCode", "Invalid Code! Try again.");
}
