using Application.Dtos.SpaceDtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface ISpaceService
{
    public Task<Result<SpaceDto>> CreateSpaceAsync(CreateSpaceDto createSpaceDto);
}
