using Application.Common.Errors;
using Application.Dtos;
using Application.Dtos.InvitationDtos;
using Application.ServiceAbstractions;
using Application.Shared.Pagination;
using Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class InvitationController(IInvitationService invitationService,
                                  IValidator<SendInvitationDto> sendInvitationValidator,
                                  IValidator<GetInvitationDto> getInvitationValidator,
                                  IValidator<QueryFilter> queryFilterValidator) : BaseApiController
{
    [Authorize(Roles = Role.Admin)]
    [HttpPost("send")]
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
    [HttpGet("validate")]
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
    [HttpPost("accept")]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseToReturnDto>> AcceptInvitation([FromQuery] string token)
    {
        var result = await invitationService.AcceptInvitationAsync(token);
        return result.Map<ActionResult<BaseToReturnDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }
    [HttpGet("by-status")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(typeof(BaseToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<InvitationDto>>> GetInvitationByStatus([FromQuery] GetInvitationDto getInvitationDto, [FromQuery] QueryFilter queryFilter)
    {
        // validation
        var queryFilterValidation = await ExecuteWithValidation(queryFilterValidator, queryFilter);
        if (!queryFilterValidation.IsSuccess)
            return HandleFailure(queryFilterValidation.ErrorsList);
        var getInvitationValidation = await ExecuteWithValidation(getInvitationValidator, getInvitationDto);
        if (!getInvitationValidation.IsSuccess)
            return HandleFailure(getInvitationValidation.ErrorsList);
        // business logic
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await invitationService.GetInvitationByStatusAsync(getInvitationDto, queryFilter, userId!);
        return result.Map<ActionResult<PagedResponse<InvitationDto>>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }

}
