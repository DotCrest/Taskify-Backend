using Application.ServiceAbstractions;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebApi.Hubs;

public class CommentHub(ILogger<CommentHub> logger,
                        ICommentService commentService,
                        IUserQuestService userQuestService) : Hub<ICommentClient>
{
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var questId = Context.GetHttpContext()?.Request.Query["questId"].ToString();

        if (string.IsNullOrEmpty(questId))
        {
            logger.LogWarning("CommentHub connection rejected - Missing questId - ConnectionId: {ConnectionId}", connectionId);
            Context.Abort();
            return;
        }

        if (int.TryParse(questId, out var parsedQuestId) == false)
        {
            logger.LogWarning("CommentHub connection rejected - Invalid questId format - ConnectionId: {ConnectionId}, QuestId: {QuestId}", connectionId, questId);
            Context.Abort();
            return;
        }

        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        var IsUserAssignedToQuest = await userQuestService.IsUserAssignedToQuest(userId!, int.Parse(questId));
        if (!IsUserAssignedToQuest)
        {
            logger.LogWarning("CommentHub connection rejected - User not assigned to quest - ConnectionId: {ConnectionId}, UserId: {UserId}, QuestId: {QuestId}", connectionId, userId, questId);
            Context.Abort();
            return;
        }
        Context.Items["questId"] = parsedQuestId;
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            logger.LogWarning(exception, "CommentHub client disconnected with exception - ConnectionId: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            logger.LogInformation("CommentHub client disconnected gracefully - ConnectionId: {ConnectionId}", Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
}
