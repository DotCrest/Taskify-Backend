namespace Application.Dtos.InvitationDtos;

public class SendInvitationDto
{
    public string ReceiverEmail { get; set; } = default!;
    public string ReceiverRole { get; set; } = default!;
    public int WorkspaceId { get; set; }
}
