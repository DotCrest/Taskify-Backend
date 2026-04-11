using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Application.Dtos.InvitationDtos;

public class GetInvitationDto
{
    [FromRoute(Name = "WorkspaceId")]
    public int WorkspaceId { get; set; }
    [FromQuery(Name = "Status")]
    public string? Status { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
}
