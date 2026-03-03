using Domain.Models;

namespace Application.Specifications.SpaceSpecifications
{
    public class SpaceByIdWithWorkspaceSpecification : BaseSpecification<Space>
    {
        public SpaceByIdWithWorkspaceSpecification(int spaceId) : base(x => x.Id == spaceId)
        {
            AddInclude(x => x.Workspace);
        }
    }
}
