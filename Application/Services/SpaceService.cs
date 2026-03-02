using Application.Dtos.SpaceDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.SpaceSpecifications;
using Application.Specifications.WorkSpaceMemberSpecifications;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class SpaceService(IUnitOfWork unitOfWork,
                          IMapper mapper) : ISpaceService
{
    private readonly IGenericRepository<Space> spaceRepo = unitOfWork.Repository<Space>();
    private readonly IGenericRepository<Workspace> workspaceRepo = unitOfWork.Repository<Workspace>();
    private readonly IGenericRepository<WorkspaceMember> workspaceMemberRepo = unitOfWork.Repository<WorkspaceMember>();
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

    public async Task<Result<PagedResponse<SpaceDto>>> GetSpacesByWorkspaceIdAsync(int workspaceId, string userId, QueryFilter queryFilter)
    {
        var workspace = await workspaceRepo.GetByIdAsync(workspaceId);
        if (workspace is null)
            return Result<PagedResponse<SpaceDto>>.Failure(WorkspaceErrors.NotFound);
        var isMember = await workspaceMemberRepo.Find(new WorkSpaceMemberSpecification(workspaceId, userId));
        if (isMember is null)
            return Result<PagedResponse<SpaceDto>>.Failure(SpaceErrors.AccessDenied);
        var specification = new SpaceSpecification(workspaceId, queryFilter);
        var countSpecification = new SpaceCountSpecification(workspaceId);

        var spaces = await spaceRepo.FindAll(specification);
        var totalRecords = await spaceRepo.CountAsync(countSpecification);

        var spacesToReturn = mapper.Map<IEnumerable<SpaceDto>>(spaces);
        return Result<PagedResponse<SpaceDto>>.Success(new PagedResponse<SpaceDto>(spacesToReturn, queryFilter.PageNumber, queryFilter.PageSize, totalRecords));
    }
    public async Task<Result<SpaceDto>> GetSpaceByIdAsync(int spaceId, string userId)
    {
        var space = await spaceRepo.GetByIdAsync(spaceId);
        if (space is null)
            return Result<SpaceDto>.Failure(SpaceErrors.NotFound);
        var isMember = await workspaceMemberRepo.Find(new WorkSpaceMemberSpecification(space.WorkspaceId, userId));
        if (isMember is null)
            return Result<SpaceDto>.Failure(SpaceErrors.AccessDenied);
        var spaceToReturn = mapper.Map<SpaceDto>(space);
        return Result<SpaceDto>.Success(spaceToReturn);
    }
}
