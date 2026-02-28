using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByStatusCountSpecification : BaseSpecification<Invitation>
{
    public InvitationByStatusCountSpecification(InvitationStatusEnum status) : base(invitation => invitation.Status == status) { }
}
