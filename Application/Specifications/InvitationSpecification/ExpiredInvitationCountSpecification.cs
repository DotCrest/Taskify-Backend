using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class ExpiredInvitationCountSpecification : BaseSpecification<Invitation>
{
    private static DateTime ExpirationThreshold = DateTime.UtcNow.AddDays(-7); // Example: Invitations older than 7 days are considered expired
    public ExpiredInvitationCountSpecification() : base(x => x.CreatedAt <= ExpirationThreshold && x.Status != InvitationStatusEnum.Accepted) { }
}
