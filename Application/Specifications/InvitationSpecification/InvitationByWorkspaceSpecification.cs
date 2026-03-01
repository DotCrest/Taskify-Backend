using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByWorkspaceSpecification : BaseSpecification<Invitation>
{
    public InvitationByWorkspaceSpecification(QueryFilter queryFilter, int workspaceId) : base(x => x.WorkspaceId == workspaceId)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Invitation>(queryFilter.SortBy);
    }
}
