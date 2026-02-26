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
        Task<Result<WorkspaceDetailsDto>> GetWorkSpaceByIdAsync(int workSpaceId, string userId);
        Task<Result<WorkspaceSimpleDto>> CreateWorkspaceAsync(CreateWorkspaceDto createWorkspaceDto, string userId);
    }
}
