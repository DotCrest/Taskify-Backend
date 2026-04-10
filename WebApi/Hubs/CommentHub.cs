using Application.Common.Errors;
using Application.Dtos.CommentDto;
using Application.ServiceAbstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WebApi.Hubs;
[Authorize]
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
        var IsUserAssignedToQuest = await userQuestService.IsUserAssignedToQuest(userId!, parsedQuestId);
        if (!IsUserAssignedToQuest)
        {
            logger.LogWarning("CommentHub connection rejected - User not assigned to quest - ConnectionId: {ConnectionId}, UserId: {UserId}, QuestId: {QuestId}", connectionId, userId, questId);
            Context.Abort();
            return;
        }
        Context.Items["questId"] = questId;
        Context.Items["userId"] = userId;
        // Store the questId as string for group operations
        await Groups.AddToGroupAsync(connectionId, questId);
        logger.LogInformation("CommentHub client connected - ConnectionId: {ConnectionId}, UserId: {UserId}, QuestId: {QuestId}", connectionId, userId, questId);
    }
    public async Task SendComment(string comment)
    {
        var questId = Context.Items["questId"] as string;
        var userId = Context.Items["userId"] as string;

        if (string.IsNullOrEmpty(questId) || string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("SendComment rejected - Missing context data - ConnectionId: {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.ReceiveErrors([new Error("MissingContextData", "Invalid session context")]);
            return;
        }

        var addCommentDto = new AddCommentDto
        {
            UserComment = comment,
            QuestId = int.Parse(questId),
            UserId = userId
        };

        var result = await commentService.AddCommentAsync(addCommentDto);


        await result.MapAsync(
            onSuccess: res =>
            {
                logger.LogInformation("Comment added successfully - CommentId: {CommentId}, QuestId: {QuestId}, UserId: {UserId}", res.Id, questId, userId);
                return Clients.Group(questId).ReceiveComment(res);
            },
            onFailure: err =>
            {
                logger.LogWarning("Failed to add comment - QuestId: {QuestId}, UserId: {UserId}, Errors: {Errors}", questId, userId, string.Join(", ", err.Select(e => e.Message)));
                return Clients.Caller.ReceiveErrors(err);
            }
        );
    }
    public async Task EditComment(UpdateCommentDto updateCommentDto)
    {
        var userId = Context.Items["userId"] as string;
        var questId = Context.Items["questId"] as string;
        updateCommentDto.UserId = userId;

        var result = await commentService.UpdateCommentAsync(updateCommentDto);
        await result.MapAsync(
            onSuccess: res => Clients.Group(questId!).ReceiveEditedComment(res),
            onFailure: err => Clients.Caller.ReceiveErrors(err)
        );
    }
    public async Task DeleteComment(int commentId)
    {
        var userId = Context.Items["userId"] as string;
        var questId = Context.Items["questId"] as string;
        var result = await commentService.DeleteCommentAsync(commentId, userId!);
        await result.MapAsync(
            onSuccess: _ => Clients.Group(questId!).ReceiveDeletedComment(commentId, "This comment has been deleted."),
            onFailure: err => Clients.Caller.ReceiveErrors(err)
        );
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
