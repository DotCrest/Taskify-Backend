using Application.Common.Errors;

namespace Application.Shared.Errors
{
    public static class QuestErrors
    {
        public static readonly Error NotFound
            = new("Quest.NotFound", "The specified quest was not found.");
        public static readonly Error CreatedFailed
            = new("Quest.CreatedFailed", "Failed to create the quest.");
        public static readonly Error UpdatedFailed
            = new("Quest.UpdatedFailed", "Failed to update the quest.");
    }
}
