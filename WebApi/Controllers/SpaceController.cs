using Application.Common.Errors;
using Application.Dtos.SpaceDtos;
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
public class SpaceController(ISpaceService spaceService,
                             IValidator<CreateSpaceDto> validator) : BaseApiController
{
    [HttpPost("create")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpaceDto>> CreateSpace(CreateSpaceDto createSpaceDto)
    {
        var validationResult = await ExecuteWithValidation(validator, createSpaceDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);
        var result = await spaceService.CreateSpaceAsync(createSpaceDto);
        if (!result.IsSuccess)
            return HandleFailure(result.ErrorsList);
        return result.Map<ActionResult<SpaceDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err));
    }
    [HttpGet("get-all")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(PagedResponse<SpaceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<SpaceDto>>> GetAllSpaces([FromQuery] int workspaceId, [FromQuery] QueryFilter queryFilter)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await spaceService.GetSpacesByWorkspaceIdAsync(workspaceId, userId!, queryFilter);
        return result.Map<ActionResult<PagedResponse<SpaceDto>>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err));
    }
}
