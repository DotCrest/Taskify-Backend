using Application.Common.Errors;

namespace Application.Shared.Errors;

public static class WorkspaceErrors
{
    public static readonly Error NotFound =
            new Error("Workspace.NotFound", "Workspace is not found!");
}
