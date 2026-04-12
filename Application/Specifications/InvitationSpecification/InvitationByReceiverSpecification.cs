using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByReceiverSpecification : BaseSpecification<Invitation>
{
    public InvitationByReceiverSpecification(string receiverEmail, int workspaceId) :
        base(x =>
            x.ReceiverEmail == receiverEmail
            && x.WorkspaceId == workspaceId
            && (x.Status == InvitationStatusEnum.Pending || x.Status == InvitationStatusEnum.Accepted)
            && x.IsActive
        )
    {
    }
}
