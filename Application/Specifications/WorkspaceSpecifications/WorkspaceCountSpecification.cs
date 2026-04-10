using Domain.Models;

namespace Application.Specifications.WorkspaceSpecifications
{
    public class WorkspaceCountSpecification : BaseSpecification<Workspace>
    {
        public WorkspaceCountSpecification(string userId)
            : base(w => w.OwnerId == userId)
        {

        }
    }
}
