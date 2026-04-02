using Domain.Models;

namespace Application.Specifications.SpaceSpecifications;

public class SpaceCountSpecification : BaseSpecification<Space>
{
    public SpaceCountSpecification(int workspaceId) : base(x => x.WorkspaceId == workspaceId) { }
}
