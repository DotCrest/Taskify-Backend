using Application.Dtos.SpaceDtos;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions;

public interface ISpaceService
{
    public Task<Result<SpaceDto>> CreateSpaceAsync(CreateSpaceDto createSpaceDto);
    public Task<Result<PagedResponse<SpaceDto>>> GetSpacesByWorkspaceIdAsync(int workspaceId, string userId, QueryFilter queryFilter);
}
