using Application.Common.Errors;
using Application.Dtos.CommentDto;

namespace WebApi.Hubs;

public interface ICommentClient
{
    Task ReceiveComment(CommentDto comment);
    Task ReceiveEditedComment(CommentDto comment);
    Task ReceiveErrors(IEnumerable<Error> errors);
    Task ReceiveDeletedComment(int commentId, string message);
}
