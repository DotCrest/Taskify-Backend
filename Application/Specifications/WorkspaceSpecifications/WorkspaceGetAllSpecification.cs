using Application.Shared.Pagination;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Specifications.WorkspaceSpecifications
{
    public class WorkspaceGetAllSpecification : BaseSpecification<Workspace>
    {
        public WorkspaceGetAllSpecification(QueryFilter queryFilter)
            :base(w => true)
        {
            AddInclude(w => w.User);
            AddInclude(w=>w.WorkspaceMembers);
            ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
            if(!string.IsNullOrEmpty(queryFilter.SortBy))
                ApplySorting<Workspace>(queryFilter.SortBy);


        }
    }
}
