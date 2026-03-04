using Application.Common.Errors;
using Application.Dtos.SpaceDtos;
using Application.Dtos.TagDtos;
using Application.ServiceAbstractions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class TagController(ITagService tagService,
                           IValidator<TagDto> tagDtoValidator) : BaseApiController
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagToReturnDto>> CreateTagAsync(TagDto tagDto)
    {
        var validationResult = await ExecuteWithValidation(tagDtoValidator, tagDto);
        if (!validationResult.IsSuccess)
            return HandleFailure(validationResult.ErrorsList);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await tagService.CreateTagAsync(tagDto, userId!);
        return result.Map<ActionResult<TagToReturnDto>>(
            onSuccess: tag => Ok(tag),
            onFailure: err => HandleFailure(err));
    }
}
