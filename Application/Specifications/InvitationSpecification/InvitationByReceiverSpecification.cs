using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByReceiverSpecification : BaseSpecification<Invitation>
{
    private static readonly DateTime ExpirationDate = DateTime.UtcNow.AddDays(-7);
    public InvitationByReceiverSpecification(string receiverEmail, int workspaceId) :
        base(x =>
            x.ReceiverEmail == receiverEmail
            && x.WorkspaceId == workspaceId
            && (x.Status == InvitationStatusEnum.Pending || x.Status == InvitationStatusEnum.Accepted)
            && x.CreatedAt >= ExpirationDate
        )
    {
    }
}
