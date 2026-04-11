using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class QuestCountSpecification : BaseSpecification<Quest>
    {
        public QuestCountSpecification(int spaceId)
            : base(q => q.SpaceId == spaceId)
        {

        }
    }
}
