using Domain.Models;

namespace Application.Dtos.InvitationDtos;

public class InvitationDto
{
    public int Id { get; set; }
    public string ReceiverEmail { get; set; } = default!;
    public InvitationStatusEnum Status { get; set; }
    public string ReceiverRole { get; set; } = default!;
}
