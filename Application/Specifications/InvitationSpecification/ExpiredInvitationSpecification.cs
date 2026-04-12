using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class ExpiredInvitationSpecification : BaseSpecification<Invitation>
{
    private static DateTime ExpirationThreshold = DateTime.UtcNow.AddDays(-7); // Example: Invitations older than 7 days are considered expired
    public ExpiredInvitationSpecification(QueryFilter queryFilter) : base(x => x.CreatedAt <= ExpirationThreshold && x.Status != InvitationStatusEnum.Accepted)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Invitation>(queryFilter.SortBy);
    }
}
