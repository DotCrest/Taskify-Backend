using Domain.Models;

namespace Application.Specifications.TagSpecifiacations;

public class TagByNameAndWorkspaceSpecification : BaseSpecification<Tag>
{
    public TagByNameAndWorkspaceSpecification(string tagName, int workspaceId)
        : base(x => x.Name.ToLower() == tagName.Trim().ToLower() && x.WorkspaceId == workspaceId)
    {
    }
}
