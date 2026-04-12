using Application.Common.Errors;

namespace Application.Shared.Errors;

internal class UserQuestErrors
{
    public readonly static Error UserAlreadyAssigned
        = new Error("UserQuest.UserAlreadyAssigned", "User is already assigned to this quest.");
    public readonly static Error UserNotAssignedToQuest
        = new Error("UserQuest.UserNotAssignedToQuest", "User is not assigned to this quest.");
}
