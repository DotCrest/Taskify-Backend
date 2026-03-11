using Application.Common.Errors;

namespace Application.Shared.Errors
{
    public static class QuestErrors
    {
        public static readonly Error NotFound
            = new("Quest.NotFound", "The specified quest was not found.");
    }
}
