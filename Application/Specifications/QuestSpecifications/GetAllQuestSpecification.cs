using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class GetAllQuestSpecification : BaseSpecification<Quest>
    {
        public GetAllQuestSpecification(QuestCustomQueryFilter queryFilter, int spaceId)
            : base(
                q =>
                (q.SpaceId == spaceId)
                &&
                (!queryFilter.DueDate.HasValue ||
                    (q.DueDate!.Value >= queryFilter.DueDate!.Value && q.DueDate.Value < queryFilter.DueDate.Value.AddDays(1)))
                &&
                (!queryFilter.CreatedAt.HasValue ||
                    (q.CreatedAt >= queryFilter.CreatedAt!.Value && q.CreatedAt < queryFilter.CreatedAt.Value.AddDays(1)))
                &&
                (!queryFilter.UpdatedAt.HasValue ||
                    (q.UpdatedAt!.Value >= queryFilter.UpdatedAt!.Value && q.UpdatedAt < queryFilter.UpdatedAt.Value.AddDays(1)))
            )
        {
            AddInclude(q => q.Category);
            AddInclude(q => q.Assignees);
            AddInclude(q => q.Tags);
            AddInclude(q => q.Author);
            ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
            if (!string.IsNullOrEmpty(queryFilter.SortBy))
                ApplySorting<Quest>(queryFilter.SortBy);
        }
    }
}
