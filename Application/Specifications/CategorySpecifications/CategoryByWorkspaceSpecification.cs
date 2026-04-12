using Domain.Models;

namespace Application.Specifications.CategorySpecifications;

public class CategoryByWorkspaceSpecification : BaseSpecification<Category>
{
    public CategoryByWorkspaceSpecification(int workspaceId) : base(x => x.WorkspaceId == workspaceId)
    {
    }
}
