using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InActiveInvitationSpecification : BaseSpecification<Invitation>
{
    private static DateTime ExpirationThreshold = DateTime.UtcNow.AddDays(-7);
    public InActiveInvitationSpecification() : base(x => x.CreatedAt <= ExpirationThreshold && x.Status != InvitationStatusEnum.Expired)
    {
    }
}
