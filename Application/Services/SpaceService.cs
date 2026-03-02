using Application.Dtos.SpaceDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class SpaceService(IUnitOfWork unitOfWork,
                          IMapper mapper) : ISpaceService
{
    private readonly IGenericRepository<Space> spaceRepo = unitOfWork.Repository<Space>();
    private readonly IGenericRepository<Workspace> workspaceRepo = unitOfWork.Repository<Workspace>();
    public async Task<Result<SpaceDto>> CreateSpaceAsync(CreateSpaceDto createSpaceDto)
    {
        var workspace = await workspaceRepo.GetByIdAsync(createSpaceDto.WorkspaceId);
        if (workspace is null)
            return Result<SpaceDto>.Failure(WorkspaceErrors.NotFound);
        var existedSpace = await spaceRepo.Find(s => s.Name == createSpaceDto.Name && s.WorkspaceId == createSpaceDto.WorkspaceId);
        if (existedSpace is not null)
            return Result<SpaceDto>.Failure(SpaceErrors.AlreadyExists);
        var space = new Space
        {
            Name = createSpaceDto.Name,
            // set default values
            IconColor = string.IsNullOrEmpty(createSpaceDto.IconColor) ? "#808080" : createSpaceDto.IconColor,
            IconType = string.IsNullOrEmpty(createSpaceDto.IconType) ? "material-symbols-outlined" : createSpaceDto.IconType,
            IconValue = string.IsNullOrEmpty(createSpaceDto.IconValue) ? "label" : createSpaceDto.IconValue,
            WorkspaceId = createSpaceDto.WorkspaceId,
            CreatedAt = DateTime.UtcNow,
        };
        await spaceRepo.AddAsync(space);
        await unitOfWork.SaveAsync();
        var spaceToReturn = mapper.Map<SpaceDto>(space);
        return Result<SpaceDto>.Success(spaceToReturn);
    }
}
