namespace Application.Dtos.SpaceDtos;

public class CreateSpaceDto
{
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = default!;
    public string? IconColor { get; set; }
    public string? IconType { get; set; }
    public string? IconValue { get; set; }
}
