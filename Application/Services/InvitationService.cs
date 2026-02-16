using Application.Common.Errors;
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
        var IsWorkspaceExist = await workSpaceService.IsWorkSpaceExsist(sendInvitationDto.WorkspaceId);
        if (!IsWorkspaceExist)
            return Result<BaseToReturnDto>.Failure(new Error("Workspace.NotFound", "Workspace is not found!")); // TODO: create a new error in Workspace Errors for this case
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
                        you have been invited by {"SENDER NAME"} to join {"WORKSPACE NAME WILL BE HERE"}.
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
}
