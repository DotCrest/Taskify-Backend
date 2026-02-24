using Application.Dtos.WorkspaceDtos;
using Application.Shared;
using Application.Shared.Pagination;
using Domain.Models;

namespace Application.ServiceAbstractions
{
    public interface IWorkSpaceService
    {
        Task<Workspace?> GetWorkSpaceById(int workSpaceId);
        Task<Result<PagedResponse<WorkspaceSimpleDto>>> GetAllWorkspacesAsync(QueryFilter queryFilter, string userId);
    }
}
