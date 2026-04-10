using Application.Dtos.SpaceDtos;
using Application.Dtos.WorkspaceMemberDtos;

namespace Application.Dtos.WorkspaceDtos;

public class WorkspaceDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? Avatar { get; set; }
    public string OwnerId { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public ICollection<SimpleSpaceDto> Spaces { get; set; } = [];
    public ICollection<WorkspaceMemberDto> Members { get; set; } = [];
    public int TotalSpaces { get; set; }
    public int TotalMembers { get; set; }
    public int TotalTags { get; set; }
}
