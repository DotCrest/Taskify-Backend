using Domain.Contracts;

namespace Domain.Models;

public class Invitation : ISoftDelete
{
    public int Id { get; set; }
    public string ReceiverEmail { get; set; } = default!;
    public InvitationStatusEnum Status { get; set; }
    public string ReceiverRole { get; set; } = default!;

    public string? SenderId { get; set; } = default!;
    public User? Sender { get; set; } = default!;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Optional Space Link
    public int? SpaceId { get; set; }
    public Space? Space { get; set; }

    // Required Workspace Link
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;
}
