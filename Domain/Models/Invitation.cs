namespace Domain.Models;

public class Invitation
{
    public int Id { get; set; }
    public string ReceiverEmail { get; set; } = default!;
    public InvitationStatusEnum Status { get; set; }
    public string ReceiverRole { get; set; } = default!;

    // Relations: One to Many => relationships with (Space, Workspace, User)
    public string? SenderId { get; set; } = default!;
    public User? Sender { get; set; } = default!;

    // Optional Space Link
    public int? SpaceId { get; set; }
    public Space? Space { get; set; }

    // Required Workspace Link
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = default!;
}
