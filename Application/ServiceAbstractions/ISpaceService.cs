using Application.Dtos.SpaceDtos;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions;

public interface ISpaceService
{
    public Task<Result<SpaceDto>> CreateSpaceAsync(CreateSpaceDto createSpaceDto);
    public Task<Result<PagedResponse<SpaceDto>>> GetSpacesByWorkspaceIdAsync(int workspaceId, string userId, QueryFilter queryFilter);
    public Task<Result<SpaceDto>> GetSpaceByIdAsync(int spaceId, string userId);
    public Task<Result<SpaceDto>> UpdateSpaceAsync(int spaceId, PatchSpaceDto updateSpaceDto, string userId);
    public Task<Result<bool>> DeleteSpaceAsync(int spaceId, string userId);
}
