using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class CommentErrors
{
    public static readonly Error AccessDenied
        = new("Comment.AccessDenied", "You can't comment on this quest because you are not assigned to it");
}
