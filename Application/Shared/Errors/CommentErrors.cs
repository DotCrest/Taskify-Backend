using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class CommentErrors
{
    public static readonly Error AccessDenied =
        new("Comment.AccessDenied", "You can't comment on this quest because you are not assigned to it");
    public static readonly Error NotFound =
        new("Comment.NotFound", "The comment with the provided id was not found");
    public static readonly Error EditTimeout =
        new("Comment.EditTimeout", "You can't edit this comment because the edit time window has expired");
}
