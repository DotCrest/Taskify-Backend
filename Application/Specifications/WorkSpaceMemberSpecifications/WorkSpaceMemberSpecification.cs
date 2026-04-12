using Domain.Models;

namespace Application.Specifications.WorkSpaceMemberSpecifications;

public class WorkSpaceMemberSpecification : BaseSpecification<WorkspaceMember>
{
    public WorkSpaceMemberSpecification(int workspaceId, string userId)
        : base(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId)
    {
        AddInclude(mw => mw.User);
    }
    public WorkSpaceMemberSpecification(int workspaceId)
        : base(wm => wm.WorkspaceId == workspaceId)
    {
        AddInclude(mw => mw.User);
    }
}