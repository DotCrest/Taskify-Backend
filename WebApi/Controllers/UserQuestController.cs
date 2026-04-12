using Application.Common.Errors;
using Application.Dtos.UserQuestDtos;
using Application.ServiceAbstractions;
using Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserQuestController(IUserQuestService userQuestService, IValidator<UserToQuestDto> userToQuestValidator) : BaseApiController
{
    [HttpPost("assign")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(typeof(UserQuestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserQuestDto>> AssignUserToQuest([FromBody] UserToQuestDto userToQuestDto)
    {
        var validationResult = await ExecuteWithValidation(userToQuestValidator, userToQuestDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);

        var assignerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        userToQuestDto.AssignerId = assignerId;

        var result = await userQuestService.AssignUserToQuest(userToQuestDto);
        return result.Map(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }
    [HttpDelete("unassign")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> UnAssignUserFromQuest([FromQuery] string userId, [FromQuery] int questId)
    {
        var result = await userQuestService.UnAssignUserFromQuestAsync(userId, questId);
        return result.Map(
            onSuccess: res => NoContent(),
            onFailure: err => HandleFailure(err)
        );
    }
}
