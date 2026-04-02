using Domain.Models;

namespace Application.Specifications.TagSpecifiacations;

public class TagByWorkspaceCountSpecification : BaseSpecification<Tag>
{
    public TagByWorkspaceCountSpecification(int workspaceId) : base(x => x.WorkspaceId == workspaceId) { }
}
