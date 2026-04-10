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
    public class QuestController(IValidator<QuestCustomQueryFilter> QuestQueryFilterValidtor,
                                 IValidator<QuestToCreateDto> CreateQuestValidator,
                                 IValidator<QuestToUpdateDto> updateQuestValidator,
                                 IQuestService questService) : BaseApiController
    {
        [HttpGet("space/{spaceid:int}")]
        [Authorize]
        [ProducesResponseType(typeof(PagedResponse<QuestToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResponse<QuestToReturnDto>>> GetAllQuests([FromQuery] QuestCustomQueryFilter queryFilter, int spaceid)
        {
            var queryFilterValidation = await ExecuteWithValidation(QuestQueryFilterValidtor, queryFilter);
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
        public async Task<ActionResult<QuestToReturnDto>> GetQuestById(int spaceId, int questId)
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
        [HttpPost("/spaces/{spaceId:int}/quests")]
        [Authorize]
        [ProducesResponseType(typeof(QuestToReturnDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<QuestToReturnDto>> CreateQuest(int spaceId, [FromBody] QuestToCreateDto createQuestDto)
        {
            var validationResult = await ExecuteWithValidation(CreateQuestValidator, createQuestDto);
            if (!validationResult.IsSuccess)
                return HandleFailure(validationResult.ErrorsList);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await questService.CreateQuestAsync(userId!, createQuestDto, spaceId);
            return result.Map(
                onSuccess: res => CreatedAtAction(nameof(GetQuestById), new { spaceId = spaceId, questId = res.Id }, res),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpPut("/spaces/{spaceId:int}/quests/{questId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> UpdateQuest(int spaceId, int questId, [FromBody] QuestToUpdateDto updateQuestDto)
        {
            var validationResult = await ExecuteWithValidation(updateQuestValidator, updateQuestDto);
            if (!validationResult.IsSuccess)
                return HandleFailure(validationResult.ErrorsList);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await questService.UpdateQuestAsync(userId, questId, spaceId, updateQuestDto);
            return result.Map(
                onSuccess: res => NoContent(),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpDelete("/spaces/{spaceId:int}/quests/{questId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> DeleteQuest(int spaceId, int questId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await questService.DeleteQuestAsync(userId, questId, spaceId);
            return result.Map(
                onSuccess: res => NoContent(),
                onFailure: err => HandleFailure(err)
            );
        }
    }
}
