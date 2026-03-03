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
                             IValidator<CreateSpaceDto> createSpaceValidator,
                             IValidator<PatchSpaceDto> patchSpaceValidator) : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpaceDto>> CreateSpace(CreateSpaceDto createSpaceDto)
    {
        var validationResult = await ExecuteWithValidation(createSpaceValidator, createSpaceDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);
        var result = await spaceService.CreateSpaceAsync(createSpaceDto);
        if (!result.IsSuccess)
            return HandleFailure(result.ErrorsList);
        return result.Map<ActionResult<SpaceDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err));
    }
    [HttpGet]
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
    [HttpGet("{spaceId}")]
    [Authorize]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceDto>> GetSpaceById(int spaceId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await spaceService.GetSpaceByIdAsync(spaceId, userId!);
        return result.Map<ActionResult<SpaceDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err));
    }
    [HttpPatch("{spaceId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceDto>> EditSpace(int spaceId, PatchSpaceDto patchSpaceDto)
    {
        var validationResult = await ExecuteWithValidation(patchSpaceValidator, patchSpaceDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await spaceService.UpdateSpaceAsync(spaceId, patchSpaceDto, userId!);
        return result.Map<ActionResult<SpaceDto>>(
            onSuccess: res => Ok(res),
            onFailure: err => HandleFailure(err));
    }
    [HttpDelete("{spaceId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteSpace(int spaceId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await spaceService.DeleteSpaceAsync(spaceId, userId!);
        return result.Map(
            onSuccess: res => NoContent(),
            onFailure: err => HandleFailure(err));

    }
}
