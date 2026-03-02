using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByTokenSpecification : BaseSpecification<Invitation>
{
    public InvitationByTokenSpecification(string token) : base(i => i.Token == token)
    {
        AddInclude(i => i.Workspace);
    }
}
