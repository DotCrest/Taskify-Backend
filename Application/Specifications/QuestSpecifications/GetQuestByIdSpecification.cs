using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class GetQuestByIdSpecification : BaseSpecification<Quest>
    {
        public GetQuestByIdSpecification(int spaceId)
            : base(q => q.SpaceId == spaceId)
        {
            AddInclude(q => q.Category);
            AddInclude(q => q.Assignees);
            AddInclude(q => q.Tags);
            AddInclude(q => q.Author);
        }
    }
}
