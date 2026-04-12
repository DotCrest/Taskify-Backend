namespace Application.Dtos.TagDtos;

public class TagDto
{
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = default!;
    public string? Color { get; set; } = default!;
}
