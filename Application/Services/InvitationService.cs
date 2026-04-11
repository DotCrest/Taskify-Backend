using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.InvitationSpecification;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Application.Services;

public class InvitationService(IUnitOfWork unitOfWork,
                           IAccountService accountService,
                           IEmailService emailService,
                           IWorkSpaceMemberService workSpaceMemberService,
                           IOptions<UrlOptions> urlOptions,
                           IMapper mapper,
                           ILogger<InvitationService> logger) : IInvitationService
{
    private readonly UrlOptions urlOptions = urlOptions.Value;
    private readonly IGenericRepository<Workspace> workspaceRepo = unitOfWork.Repository<Workspace>();
    private readonly IGenericRepository<Invitation> invitationRepo = unitOfWork.Repository<Invitation>();
    public async Task<Result<PagedResponse<InvitationDto>>> GetAllInvitationsAsync(QueryFilter queryFilter, GetInvitationDto getInvitationDto)
    {
        var specification = new InvitationByWorkspaceSpecification(queryFilter, getInvitationDto);
        var countSpecification = new InvitationByWorkspaceCountSpecification(getInvitationDto);

        var workspace = await workspaceRepo.GetByIdAsync(getInvitationDto.WorkspaceId);
        if (workspace is null)
            return Result<PagedResponse<InvitationDto>>.Failure(WorkspaceErrors.NotFound);

        if (workspace.OwnerId != getInvitationDto.UserId)
            return Result<PagedResponse<InvitationDto>>.Failure(WorkspaceErrors.AccessDenied);

        var invitations = await invitationRepo.FindAll(specification);
        var invitationsCount = await invitationRepo.CountAsync(countSpecification);

        var invitationDtos = mapper.Map<IEnumerable<InvitationDto>>(invitations);
        var pagedResponse = new PagedResponse<InvitationDto>(invitationDtos, queryFilter.PageNumber, queryFilter.PageSize, invitationsCount);
        return Result<PagedResponse<InvitationDto>>.Success(pagedResponse);
    }
    public async Task<Result<BaseToReturnDto>> SendInvitationAsync(SendInvitationDto sendInvitationDto, string senderId)
    {
        // check if the sender exists
        var sender = await accountService.GetUserByIdAsync(senderId);
        if (sender is null)
            return Result<BaseToReturnDto>.Failure(AuthErrors.UserNotFound);
        // check if the workspace exists
        var workspace = await workspaceRepo.GetByIdAsync(sendInvitationDto.WorkspaceId);
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
        var registerByInvitationUrl = $"{urlOptions.FrontendUrl}/frontend-page?token={invitation.Token}";
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
        var workspace = await workspaceRepo.GetByIdAsync(invitation.WorkspaceId);
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
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await workSpaceMemberService.AddWorkSpaceMemberAsync(workspaceMember);
            UpdateInvitationStatus(invitation, InvitationStatusEnum.Accepted);
            await unitOfWork.SaveAsync();
        });
        // return the result
        var result = new BaseToReturnDto { IsSuccess = true, Message = "Invitation accepted successfully!" };
        return Result<BaseToReturnDto>.Success(result);
    }
    public async Task<Result<Invitation>> GetValidInvitationAsync(string token)
    {
        var invitation = await GetInvitationByToken(token);

        if (invitation is null)
            return Result<Invitation>.Failure(InvitationErrors.NotFound);
        if (!invitation.IsActive)
        {
            UpdateInvitationStatus(invitation, InvitationStatusEnum.Expired);
            return Result<Invitation>.Failure(InvitationErrors.Expired);
        }
        if (invitation.Status == InvitationStatusEnum.Accepted)
            return Result<Invitation>.Failure(InvitationErrors.AlreadyAccepted);

        return Result<Invitation>.Success(invitation);
    }
    public void UpdateInvitationStatus(Invitation invitation, InvitationStatusEnum status)
    {
        invitation.Status = status;
        invitationRepo.Update(invitation);
    }
    public async Task PeriodicUpdateOfExpiredInvitationsAsync()
    {
        var specification = new InActiveInvitationSpecification();
        await invitationRepo.BulkUpdateAsync(specification, inv => inv.SetProperty(x => x.Status, InvitationStatusEnum.Expired));
    }
    public async Task BulkDeleteInvitationsByCriteria(Expression<Func<Invitation, bool>> criteria)
    {
        if (criteria is null)
        {
            logger.LogError("BulkDeleteInvitationsByCriteria: criteria is null");
            throw new ArgumentNullException(nameof(criteria));
        }
        if (criteria.Body is ConstantExpression constant && (bool)constant.Value! == true)
        {
            logger.LogError("BulkDeleteInvitationsByCriteria: criteria is too broad and may lead to deleting all invitations");
            throw new Exception();
        }
        await invitationRepo.BulkDeleteAsync(criteria);
    }
    public async Task<Result<bool>> DeleteInvitationById(int invitationId)
    {
        var invitation = await invitationRepo.GetByIdAsync(invitationId);
        if (invitation is null)
            return Result<bool>.Failure(InvitationErrors.NotFound);
        invitationRepo.Delete(invitation);
        await unitOfWork.SaveAsync();
        return Result<bool>.Success(true);
    }
    private async Task<Invitation?> GetInvitationByToken(string token)
    {
        var specification = new InvitationByTokenSpecification(token);
        var invitation = await invitationRepo.Find(specification);
        return invitation;
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
