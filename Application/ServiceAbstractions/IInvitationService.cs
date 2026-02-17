using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface IInvitationService
{
    Task<Result<BaseToReturnDto>> SendInvitationAsync(SendInvitationDto sendInvitationDto, string senderId);
    Task<Result<InviteValidationDto>> ValidateInvitationAsync(string token);
    Task<Result<BaseToReturnDto>> AcceptInvitationAsync(string token);
}
