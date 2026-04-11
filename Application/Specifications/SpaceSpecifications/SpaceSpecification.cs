using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.SpaceSpecifications;

public class SpaceSpecification : BaseSpecification<Space>
{
    public SpaceSpecification(int workspaceId, QueryFilter queryFilter) : base(x => x.WorkspaceId == workspaceId)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Space>(queryFilter.SortBy);
    }
}
