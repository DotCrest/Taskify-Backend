using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class GetAllQuestSpecification : BaseSpecification<Quest>
    {
        public GetAllQuestSpecification(QueryFilter queryFilter, int spaceId)
            : base(q => q.SpaceId == spaceId)
        {
            AddInclude(q => q.Category);

            AddInclude(q => q.Tags);
            AddInclude(q => q.Author);
            ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
            if (!string.IsNullOrEmpty(queryFilter.SortBy))
                ApplySorting<Quest>(queryFilter.SortBy);
        }
    }
}
