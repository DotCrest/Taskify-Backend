using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.WorkspaceSpecifications
{
    public class WorkspaceGetAllSpecification : BaseSpecification<Workspace>
    {
        public WorkspaceGetAllSpecification(QueryFilter queryFilter, string userId)
            : base(w => w.OwnerId == userId)
        {
            AddInclude(w => w.User);
            AddInclude(w => w.WorkspaceMembers);
            ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
            if (!string.IsNullOrEmpty(queryFilter.SortBy))
                ApplySorting<Workspace>(queryFilter.SortBy);


        }
    }
}
