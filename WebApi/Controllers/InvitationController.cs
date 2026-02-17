using Application.Common.Errors;
using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.ServiceAbstractions;
using Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class InvitationController(IInvitationService invitationService,
                                  IValidator<SendInvitationDto> sendInvitationValidator) : BaseApiController
{
    [Authorize(Roles = Role.Admin)]
    [HttpPost("send-invitation")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseToReturnDto>> SendInvitation(SendInvitationDto sendInvitationDto)
    {
        // validation
        var validation = await ExecuteWithValidation(sendInvitationValidator, sendInvitationDto);
        if (!validation.IsSuccess)
            return HandleFailure(validation.ErrorsList);
        // business logic
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await invitationService.SendInvitationAsync(sendInvitationDto, senderId!);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }
    [HttpGet("validate-invitation")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InviteValidationDto>> ValidateInvitation([FromQuery] string token)
    {
        var result = await invitationService.ValidateInvitationAsync(token);
        return result.Map<ActionResult<InviteValidationDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }
}
