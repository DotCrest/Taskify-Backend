using Domain.Models;

namespace Application.Specifications.WorkspaceSpecifications
{
    public class WorkspaceGetByIdSpecification : BaseSpecification<Workspace>
    {
        public WorkspaceGetByIdSpecification(int workspaceId, string userId)
            : base(w => w.Id == workspaceId && w.OwnerId == userId || w.WorkspaceMembers.Any(m => m.UserId == userId))
        {
            AddInclude(w => w.WorkspaceMembers);
            AddInclude(w => w.User);
            AddInclude(w => w.Spaces);
            AddInclude(w => w.Tags);

        }
    }
}
