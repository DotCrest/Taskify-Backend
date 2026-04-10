using Application.Dtos.CommentDto;
using Application.ServiceAbstractions;
using Application.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController(ICommentService commentService) : BaseApiController
{
    [HttpGet("{questId}")]
    [ProducesResponseType(typeof(PagedResponse<CommentDto>), StatusCodes.Status200OK)]

    // TODO: add caching to this endpoint
    public async Task<ActionResult<PagedResponse<CommentDto>>> GetComments(int questId, [FromQuery] QueryFilter queryFilter)
    {
        var result = await commentService.GetCommentsByQuestIdAsync(questId, queryFilter);
        return result.Map<ActionResult<PagedResponse<CommentDto>>>(
               onSuccess: comments => Ok(comments),
               onFailure: _ => HandleFailure(_));
    }
}
