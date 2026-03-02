using Domain.Models;

namespace Application.Specifications.QuestSpecifications
{
    public class QuestsByWorkspaceSpecification : BaseSpecification<Quest>
    {
        public QuestsByWorkspaceSpecification(int workspaceId)
            : base(q => q.Category.WorkspaceId == workspaceId)
        {

        }
    }
}
