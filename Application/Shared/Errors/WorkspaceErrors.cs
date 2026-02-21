using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class WorkspaceErrors
{
    public static readonly Error NotFound =
            new Error("Workspace.NotFound", "Workspace is not found!");
    public static readonly Error AccessDenied =
       new Error("Workspace.AccessDenied", "You don't have access to this workspace!");
}
