using Application.Common.Errors;
using Application.Dtos.SpaceDtos;
using Application.ServiceAbstractions;
using Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}
