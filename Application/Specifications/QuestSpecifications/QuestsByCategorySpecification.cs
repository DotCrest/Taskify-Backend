using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class QuestsByCategorySpecification : BaseSpecification<Quest>
    {
        public QuestsByCategorySpecification(List<int> categoryIds) : base(q => q.CategoryId != (int?)null && categoryIds.Contains((int)q.CategoryId))
        {

        }
    }
}
