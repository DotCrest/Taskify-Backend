using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.TagSpecifiacations;

public class TagByWorkspaceSpecification : BaseSpecification<Tag>
{
    public TagByWorkspaceSpecification(QueryFilter queryFilter, int workspaceId) : base(x => x.WorkspaceId == workspaceId)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Tag>(queryFilter.SortBy);
    }
}
