using Application.Common.Errors;
using Application.Dtos.QuestDtos;
using Application.ServiceAbstractions;
using Application.Shared.Pagination;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class QuestController(IValidator<QueryFilter> validator, IQuestService questService) : BaseApiController
    {
        [HttpGet("space/{spaceid:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PagedResponse<QuestToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<QuestToReturnDto>>> GetAll([FromQuery] QueryFilter queryFilter, int spaceid)
        {
            var queryFilterValidation = await ExecuteWithValidation(validator, queryFilter);
            if (!queryFilterValidation.IsSuccess)
                return HandleFailure(queryFilterValidation.ErrorsList);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await questService.GetAllQuests(userId!, spaceid, queryFilter);
            return result.Map(
                onSuccess: res => Ok(res),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpGet("/spaces/{spaceId:int}/quests/{questId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(QuestToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<QuestToReturnDto>> GetById(int spaceId, int questId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await questService.GetQuestByIdAsync(userId!, questId, spaceId);
            return result.Map(
                onSuccess: res => Ok(res),
                onFailure: err => HandleFailure(err)
            );
        }
    }
}
