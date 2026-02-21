using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByStatusSpecification : BaseSpecification<Invitation>
{
    public InvitationByStatusSpecification(InvitationStatusEnum status, QueryFilter queryFilter) : base(invitation => invitation.Status == status)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Invitation>(queryFilter.SortBy);
    }
}
