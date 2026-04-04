using Application.Common.Errors;
using Application.Dtos.UserQuestDtos;
using Application.ServiceAbstractions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserQuestController(IUserQuestService userQuestService, IValidator<UserToQuestDto> userToQuestValidator) : BaseApiController
{
    [HttpPost("assign")]
    [Authorize]
    [ProducesResponseType(typeof(UserQuestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserQuestDto>> AssignUserToQuest([FromBody] UserToQuestDto userToQuestDto)
    {
        var validationResult = await ExecuteWithValidation(userToQuestValidator, userToQuestDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);

        var result = await userQuestService.AssignUserToQuest(userToQuestDto);
        return result.Map(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err)
        );
    }
    [HttpDelete("unassign")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<bool>> UnAssignUserFromQuest([FromBody] UserToQuestDto userToQuestDto)
    {
        var validationResult = await ExecuteWithValidation(userToQuestValidator, userToQuestDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);

        var result = await userQuestService.UnAssignUserFromQuestAsync(userToQuestDto);
        return result.Map(
            onSuccess: res => NoContent(),
            onFailure: err => HandleFailure(err)
        );
    }
}
