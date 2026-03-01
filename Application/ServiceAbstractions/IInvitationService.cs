using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.Shared;
using Application.Shared.Pagination;
using Domain.Models;
using System.Linq.Expressions;

namespace Application.ServiceAbstractions;

public interface IInvitationService
{
    Task<Result<BaseToReturnDto>> SendInvitationAsync(SendInvitationDto sendInvitationDto, string senderId);
    Task<Result<InviteValidationDto>> ValidateInvitationAsync(string token);
    Task<Result<BaseToReturnDto>> AcceptInvitationAsync(string token);
    Task<Result<Invitation>> GetValidInvitationAsync(string token);
    Task<Result<PagedResponse<InvitationDto>>> GetInvitationByStatusAsync(GetInvitationDto getInvitationDto, QueryFilter queryFilter, string userId);
    Task PeriodicUpdateOfExpiredInvitationsAsync();
    void UpdateInvitationStatus(Invitation invitation, InvitationStatusEnum status);
    Task BulkDeleteInvitationsByCriteria(Expression<Func<Invitation, bool>> criteria);
}
