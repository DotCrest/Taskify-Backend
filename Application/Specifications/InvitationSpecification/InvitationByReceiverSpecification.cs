using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByReceiverSpecification : BaseSpecification<Invitation>
{
    public InvitationByReceiverSpecification(string receiverEmail, int workspaceId, InvitationStatusEnum invitationStatus) :
        base(x =>
            x.ReceiverEmail == receiverEmail
            && x.WorkspaceId == workspaceId
            && x.Status == invitationStatus)
    {
    }
}
