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
        IValidator<QueryFilter> queryFilterValidator) : BaseApiController
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
                onFailure: err => HandleFailure(err));
        }
    }
}
