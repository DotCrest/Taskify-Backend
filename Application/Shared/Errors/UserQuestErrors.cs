using Application.Common.Errors;

namespace Application.Shared.Errors;

internal class UserQuestErrors
{
    public static Error UserAlreadyAssigned => new Error("UserQuest.UserAlreadyAssigned", "User is already assigned to this quest.");
}
