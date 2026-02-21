namespace Application.Dtos.InvitationDtos;

public class InviteValidationDto
{
    public int WorkspaceId { get; set; }
    public string WorkspaceName { get; set; } = default!;
    public bool IsUserRegistered { get; set; }
    public string ReceiverEmail { get; set; } = default!;
}
