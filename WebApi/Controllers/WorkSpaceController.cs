using Application.Common.Errors;
using Application.Dtos.WorkspaceDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Pagination;
using Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkSpaceController(IWorkSpaceService workSpaceService,
        IValidator<QueryFilter> queryFilterValidator, IValidator<CreateWorkspaceDto> createWorkspaceValidator
        , IValidator<UpdateWorkspaceDto> updateWorkspaceValidator) : BaseApiController
    {
        [HttpGet("get-all")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<Result<PagedResponse<WorkspaceSimpleDto>>>> GetAllWorkSpaces([FromQuery] QueryFilter queryFilter)
        {
            var queryFilterValidation = await ExecuteWithValidation(queryFilterValidator, queryFilter);
            if (!queryFilterValidation.IsSuccess)
                return HandleFailure(queryFilterValidation.ErrorsList);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await workSpaceService.GetAllWorkspacesAsync(queryFilter, userId);
            return result.Map(
                onSuccess: res => Ok(res),
                onFailure: err => HandleFailure(err));

        }
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(WorkspaceDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<Result<WorkspaceDetailsDto>>> GetWorkSpaceById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await workSpaceService.GetWorkSpaceByIdAsync(id, userId);
            return result.Map(
                onSuccess: res => Ok(res),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpPost("create-workspace")]
        [Authorize(Roles = Role.Admin)]
        [ProducesResponseType(typeof(WorkspaceSimpleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<Error>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<WorkspaceSimpleDto>>> CreateWorkspace([FromBody] CreateWorkspaceDto dto)
        {
            var dtoValidator = await ExecuteWithValidation(createWorkspaceValidator, dto);
            if (!dtoValidator.IsSuccess)
                return HandleFailure(dtoValidator.ErrorsList);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await workSpaceService.CreateWorkspaceAsync(dto, userId);
            return result.Map(
                onSuccess: res => CreatedAtAction(actionName: nameof(GetWorkSpaceById)
                , routeValues: new { id = res.Id }
                , value: res),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<bool>>> UpdateWorkSpace(int id, [FromBody] UpdateWorkspaceDto dto)
        {
            var dtoValidator = await ExecuteWithValidation(updateWorkspaceValidator, dto);
            if (!dtoValidator.IsSuccess)
                return HandleFailure(dtoValidator.ErrorsList);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await workSpaceService.UpdateWorkSpaceAsync(id, dto, userId);
            return result.Map(
                onSuccess: res => NoContent(),
                onFailure: err => HandleFailure(err)
            );
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Result<bool>>> DeleteWorkSpace(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var result = await workSpaceService.DeleteWorkSpaceAsync(id, userId);
            return result.Map(
                onSuccess: res => NoContent(),
                onFailure: err => HandleFailure(err)
            );
        }
    }
}
