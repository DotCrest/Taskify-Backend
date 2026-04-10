using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class WorkspaceErrors
{
    public static readonly Error NotFound =
            new Error("Workspace.NotFound", "Workspace is not found!");
    public static readonly Error AccessDenied =
       new Error("Workspace.AccessDenied", "You don't have access to this workspace!");
    public static readonly Error CreatedFailed =
        new Error("Workspace.CreatedFailed", "An error occurred while creating the workspace");
    public static readonly Error UpdateFailed =
        new Error("Workspace.UpdateFailed", "An error occurred while updating the workspace");
    public static readonly Error DeleteFailed =
        new Error("Workspace.DeleteFailed", "An error occurred while deleting the workspace");
    public static readonly Error UserNotInWorkspace =
        new Error("Workspace.UserNotInWorkspace", "The user is not a member of the workspace");
}
