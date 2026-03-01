using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByWorkspaceCountSpecification : BaseSpecification<Invitation>
{
    public InvitationByWorkspaceCountSpecification(int workspaceId) : base(x => x.WorkspaceId == workspaceId) { }
}
