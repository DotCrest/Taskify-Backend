using Application.Dtos.InvitationDtos;
using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.InvitationSpecification;

public class InvitationByWorkspaceSpecification : BaseSpecification<Invitation>
{
    public InvitationByWorkspaceSpecification(QueryFilter queryFilter, GetInvitationDto getInvitationDto) :
        base(
            x => x.WorkspaceId == getInvitationDto.WorkspaceId
            &&
            (
                string.IsNullOrEmpty(getInvitationDto.Status) ||
                Enum.Parse<InvitationStatusEnum>(getInvitationDto.Status, true) == x.Status
            )
            )
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
        if (!string.IsNullOrEmpty(queryFilter.SortBy))
            ApplySorting<Invitation>(queryFilter.SortBy);
    }
}
