using Domain.Models;

namespace Application.Specifications.WorkspaceSpecifications
{
    public class WorkspaceGetByIdSpecification : BaseSpecification<Workspace>
    {
        public WorkspaceGetByIdSpecification(int workspaceId)
            : base(w => w.Id == workspaceId)
        {
            AddInclude(w => w.WorkspaceMembers);
            AddInclude(w => w.User);
            AddInclude(w => w.Spaces);
            AddInclude(w => w.Tags);

        }
    }
}
