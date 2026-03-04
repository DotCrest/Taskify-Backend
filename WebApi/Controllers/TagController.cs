using Application.Common.Errors;
using Application.Dtos.TagDtos;
using Application.ServiceAbstractions;
using Application.Shared.Pagination;
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
    [ProducesResponseType(typeof(TagToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
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
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(TagToReturnDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TagToReturnDto>>> GetAllTagsAsync([FromQuery] QueryFilter queryFilter, [FromQuery] int workspaceId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await tagService.GetAllTagsAsync(queryFilter, workspaceId, userId!);
        return result.Map<ActionResult<PagedResponse<TagToReturnDto>>>(
            onSuccess: tags => Ok(tags),
            onFailure: err => HandleFailure(err));
    }
    [HttpGet("{tagId}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(TagToReturnDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TagToReturnDto>> GetTagByIdAsync(int tagId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await tagService.GetTagByIdAsync(tagId, userId!);
        return result.Map<ActionResult<TagToReturnDto>>(
            onSuccess: tag => Ok(tag),
            onFailure: err => HandleFailure(err));
    }
}
