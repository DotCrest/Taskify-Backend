using Application.Dtos.InvitationDtos;
using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByWorkspaceCountSpecification : BaseSpecification<Invitation>
{
    public InvitationByWorkspaceCountSpecification(GetInvitationDto getInvitationDto) :
        base(
            x => x.WorkspaceId == getInvitationDto.WorkspaceId
            &&
            (
                string.IsNullOrEmpty(getInvitationDto.Status) ||
                Enum.Parse<InvitationStatusEnum>(getInvitationDto.Status, true) == x.Status
            )
            )
    { }
}
