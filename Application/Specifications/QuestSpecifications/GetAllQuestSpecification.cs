using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class GetAllQuestSpecification : BaseSpecification<Quest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllQuestSpecification"/> class.
        /// Performs advanced filtering on quests based on the following criteria:
        /// <list type="bullet">
        /// <item>Mandatory filtering by <paramref name="spaceId"/>.</item>
        /// <item>Optional filtering by <c>CategoryId</c> , <c>TagId</c>,  <c>Priority</c> and <c>Status</c> (using <c>Any</c> for many-to-many tag relationships).</item>
        /// <item>Optional date-range filtering for <c>DueDate</c>, <c>CreatedAt</c>, and <c>UpdatedAt</c>, isolated to the specific calendar day provided.</item>
        /// </list>
        /// The specification ensures eager loading of related entities (Category, Assignees, Tags, and Author) 
        /// and applies server-side pagination and dynamic sorting.
        /// </summary>
        /// <param name="queryFilter">The data transfer object containing filter values, sorting preferences, and pagination settings.</param>
        /// <param name="spaceId">The unique identifier of the space context.</param>
        public GetAllQuestSpecification(QuestCustomQueryFilter queryFilter, int spaceId)
            : base(
                q =>
                (q.SpaceId == spaceId)
                &&
                (queryFilter.TagId == null ||
                    q.Tags.Any(t => t.Id == queryFilter.TagId)
                )
                &&
                (queryFilter.CategoryId == null ||
                    q.CategoryId == queryFilter.CategoryId
                )
                &&
                (queryFilter.Status == null ||
                    q.Status == Enum.Parse<QuestStatusEnum>(queryFilter.Status, true)
                )
                &&
                (queryFilter.Priority == null ||
                    q.Priority == Enum.Parse<PriorityEnum>(queryFilter.Priority, true)
                )
                &&
                (!queryFilter.DueDate.HasValue ||
                    (q.DueDate!.Value.Date >= queryFilter.DueDate!.Value.Date && q.DueDate.Value.Date < queryFilter.DueDate.Value.AddDays(1)))
                &&
                (!queryFilter.CreatedAt.HasValue ||
                    (q.CreatedAt.Date >= queryFilter.CreatedAt!.Value && q.CreatedAt.Date < queryFilter.CreatedAt.Value.AddDays(1)))
                &&
                (!queryFilter.UpdatedAt.HasValue ||
                    (q.UpdatedAt!.Value.Date >= queryFilter.UpdatedAt!.Value.Date && q.UpdatedAt.Value.Date < queryFilter.UpdatedAt.Value.AddDays(1)))
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
