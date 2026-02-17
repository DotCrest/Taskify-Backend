using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Specifications.InvitationSpecification;
using Domain.Contracts;
using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class InvitationService(IUnitOfWork unitOfWork,
                           IAccountService accountService,
                           IEmailService emailService,
                           IWorkSpaceService workSpaceService,
                           IWorkSpaceMemberService workSpaceMemberService,
                           IOptions<UrlOptions> urlOptions) : IInvitationService
{
    private readonly UrlOptions urlOptions = urlOptions.Value;
    private readonly IGenericRepository<Invitation> invitationRepo = unitOfWork.Repository<Invitation>();
    public async Task<Result<BaseToReturnDto>> SendInvitationAsync(SendInvitationDto sendInvitationDto, string senderId)
    {
        // check if the sender exists
        var sender = await accountService.GetUserByIdAsync(senderId);
        if (sender is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.UserNotFound);
        // check if the workspace exists
        var workspace = await workSpaceService.GetWorkSpaceById(sendInvitationDto.WorkspaceId);
        if (workspace is null)
            return Result<BaseToReturnDto>.Failure(WorkspaceErrors.NotFound);
        // if the sender has already sent an invitation to the same email and it's still pending
        var specification = new InvitationByReceiverSpecification(sendInvitationDto.ReceiverEmail, sendInvitationDto.WorkspaceId, InvitationStatusEnum.Pending);
        var existingInvitation = await invitationRepo.Find(specification);
        if (existingInvitation is not null)
            return Result<BaseToReturnDto>.Failure(InvitationErrors.AlreadySent);
        // create invitation
        var invitation = await CreateInvitation(sendInvitationDto, senderId);
        // send email to the receiver
        var registerByInvitationUrl = $"{urlOptions.BaseUrl}/frontend-page?token={invitation.Token}";
        var receiverName = sendInvitationDto.ReceiverEmail.Split('@')[0];
        var subject = "You have been invited to join a workspace!";
        var body = $@"
            <div style=""max-width: 600px; margin: 0 auto; padding: 40px 20px; text-align: left; direction: ltr;"">
        
                    <h1 style=""font-size: 28px; font-weight: 800; color: #000000; margin-bottom: 24px; margin-top: 0;"">
                        Hi, {receiverName}!
                    </h1>

                    <p style=""font-size: 18px; line-height: 1.6; color: #475569; margin-bottom: 32px;"">
                        you have been invited by {sender.Name} to join {workspace.Name}.
                        Click the button below to set up your account and get started:
                    </p>

                    <div style=""text-align: center; margin-top: 40px;"">
                        <a href=""{registerByInvitationUrl}"" style=""background-color: #0052ea; color: #ffffff; padding: 14px 32px; text-decoration: none; font-size: 18px; font-weight: bold; border-radius: 6px; display: inline-block; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                            Set up account
                        </a>
                    </div>
            </div>";

        await emailService.SendEmailAsync(sendInvitationDto.ReceiverEmail, subject, body);
        // return the result
        return Result<BaseToReturnDto>.Success(new BaseToReturnDto
        {
            IsSuccess = true,
            Message = "Invitation sent successfully!"
        });
    }
    public async Task<Result<InviteValidationDto>> ValidateInvitationAsync(string token)
    {
        var invitationResult = await GetValidInvitationAsync(token);
        if (!invitationResult.IsSuccess)
            return Result<InviteValidationDto>.Failure(invitationResult.ErrorsList);
        var invitation = invitationResult.Value!;
        var inviteValidationDto = new InviteValidationDto
        {
            ReceiverEmail = invitation.ReceiverEmail,
            WorkspaceId = invitation.WorkspaceId,
            WorkspaceName = invitation.Workspace.Name
        };
        // check if user registered with the email of the invitation
        var user = await accountService.GetUserByEmailAsync(invitation.ReceiverEmail);
        if (user is null)
        {
            inviteValidationDto.IsUserRegistered = false;
            return Result<InviteValidationDto>.Success(inviteValidationDto);
        }
        inviteValidationDto.IsUserRegistered = true;
        // return the result
        return Result<InviteValidationDto>.Success(inviteValidationDto);
    }
    public async Task<Result<BaseToReturnDto>> AcceptInvitationAsync(string token)
    {
        // get the invitation by the token
        var invitationResult = await GetValidInvitationAsync(token);
        if (!invitationResult.IsSuccess)
            return Result<BaseToReturnDto>.Failure(invitationResult.ErrorsList);
        var invitation = invitationResult.Value!;
        // check if user exists
        var user = await accountService.GetUserByEmailAsync(invitation.ReceiverEmail);
        if (user is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.UserNotFound);
        // check if workspace exists
        var workspace = await workSpaceService.GetWorkSpaceById(invitation.WorkspaceId);
        if (workspace is null)
            return Result<BaseToReturnDto>.Failure(WorkspaceErrors.NotFound);
        // add the user to the workspace members
            var workspaceMember = new WorkspaceMember
            {
                UserId = user.Id,
                WorkspaceId = invitation.WorkspaceId,
                Role = invitation.ReceiverRole,
                JoinedAt = DateTime.UtcNow
            };
            await workSpaceMemberService.AddWorkSpaceMemberAsync(workspaceMember);
            await UpdateInvitationStatus(invitation, InvitationStatusEnum.Accepted);
        // return the result
        var result = new BaseToReturnDto { IsSuccess = true, Message = "Invitation accepted successfully!" };
        return Result<BaseToReturnDto>.Success(result);
    }
    private async Task<Invitation?> GetInvitationByToken(string token)
    {
        var specification = new InvitationByTokenSpecification(token);
        var invitation = await invitationRepo.Find(specification);
        return invitation;
    }
    private async Task UpdateInvitationStatus(Invitation invitation, InvitationStatusEnum status)
    {
        invitation.Status = status;
        invitationRepo.Update(invitation);
        await unitOfWork.SaveAsync();
    }
    private async Task<Invitation> CreateInvitation(SendInvitationDto sendInvitationDto, string senderId)
    {
        var token = Guid.NewGuid().ToString();
        var invitation = new Invitation
        {
            ReceiverEmail = sendInvitationDto.ReceiverEmail,
            ReceiverRole = sendInvitationDto.ReceiverRole,
            WorkspaceId = sendInvitationDto.WorkspaceId,
            SenderId = senderId,
            CreatedAt = DateTime.UtcNow,
            Status = InvitationStatusEnum.Pending,
            Token = token
        };
        await invitationRepo.AddAsync(invitation);
        // save the invitation in the database
        await unitOfWork.SaveAsync();
        return invitation;
    }
    public async Task<Result<Invitation>> GetValidInvitationAsync(string token)
    {
        var invitation = await GetInvitationByToken(token);

        if (invitation is null)
            return Result<Invitation>.Failure(InvitationErrors.NotFound);
        if (!invitation.IsActive)
        {
            await UpdateInvitationStatus(invitation, InvitationStatusEnum.Expired);
            return Result<Invitation>.Failure(InvitationErrors.Expired);
        }
        if (invitation.Status == InvitationStatusEnum.Accepted)
            return Result<Invitation>.Failure(InvitationErrors.AlreadyAccepted);

        return Result<Invitation>.Success(invitation);
    }
}
