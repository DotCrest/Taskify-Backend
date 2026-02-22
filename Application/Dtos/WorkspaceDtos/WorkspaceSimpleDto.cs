namespace Application.Dtos.WorkspaceDtos
{
    public class WorkspaceSimpleDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerName { get; set; } = null!;
        public int MembersCount { get; set; }
    }
}
