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
    }
}
