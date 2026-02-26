using Application.Dtos.WorkspaceDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.WorkspaceSpecifications;
using AutoMapper;
using Domain.Constants;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class WorkSpaceService(IUnitOfWork unitOfWork,
        IMapper mapper, IWorkSpaceMemberService workSpaceMemberService) : IWorkSpaceService
    {
        private readonly IGenericRepository<Workspace> repo = unitOfWork.Repository<Workspace>();


        public async Task<Result<PagedResponse<WorkspaceSimpleDto>>> GetAllWorkspacesAsync(QueryFilter queryFilter, string userId)
        {
            var workspaceCountSpec = new WorkspaceCountSpecification(userId);
            var specification = new WorkspaceGetAllSpecification(queryFilter, userId);
            var repo = unitOfWork.Repository<Workspace>();
            var totalRecords = await repo.CountAsync(workspaceCountSpec);
            var workspaces = await repo.FindAll(specification);
            var result = mapper.Map<IReadOnlyList<WorkspaceSimpleDto>>(workspaces);

            var pagedResponse = new PagedResponse<WorkspaceSimpleDto>(result, queryFilter.PageNumber, queryFilter.PageSize, totalRecords);
            return Result<PagedResponse<WorkspaceSimpleDto>>.Success(pagedResponse);

        }

        public async Task<Workspace?> GetWorkSpaceById(int workSpaceId)
        {
            var workspace = await repo.GetByIdAsync(workSpaceId);
            return workspace;
        }

        public async Task<Result<WorkspaceDetailsDto>> GetWorkSpaceByIdAsync(int workSpaceId, string userId)
        {
            var repo = unitOfWork.Repository<Workspace>();
            var specification = new WorkspaceGetByIdSpecification(workSpaceId);
            var workspace = await repo.Find(specification);
            if (workspace == null)
            {
                return Result<WorkspaceDetailsDto>.Failure(WorkspaceErrors.NotFound);
            }
            var isOwner = workspace.OwnerId == userId;
            var isMember = workspace.WorkspaceMembers.Any(m => m.UserId == userId);
            if (!isOwner && !isMember)
            {
                return Result<WorkspaceDetailsDto>.Failure(WorkspaceErrors.AccessDenied);
            }
            var result = mapper.Map<WorkspaceDetailsDto>(workspace);
            return Result<WorkspaceDetailsDto>.Success(result);
        }
        public async Task<Result<WorkspaceSimpleDto>> CreateWorkspaceAsync(CreateWorkspaceDto createWorkspaceDto, string userId)
        {
            Workspace? workspace = null;
            try
            {
                await unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    workspace = new Workspace()
                    {
                        Name = createWorkspaceDto.Name,
                        Avatar = createWorkspaceDto.Avatar,
                        CreatedAt = DateTime.UtcNow,
                        OwnerId = userId,
                    };

                    await repo.AddAsync(workspace);
                    await unitOfWork.SaveAsync();

                    var member = new WorkspaceMember()
                    {
                        WorkspaceId = workspace.Id,
                        UserId = userId,
                        JoinedAt = DateTime.UtcNow,
                        Role = Role.Admin,
                    };

                    await workSpaceMemberService.AddWorkSpaceMemberAsync(member);
                    await unitOfWork.SaveAsync();
                });


                var spec = new WorkspaceGetByIdSpecification(workspace!.Id);
                var fullWorkspace = await repo.Find(spec);

                if (fullWorkspace == null)
                {
                    return Result<WorkspaceSimpleDto>.Failure(WorkspaceErrors.CreatedFailed);
                }


                var workspaceDto = mapper.Map<WorkspaceSimpleDto>(fullWorkspace);

                return Result<WorkspaceSimpleDto>.Success(workspaceDto);
            }
            catch (Exception ex)
            {

                return Result<WorkspaceSimpleDto>.Failure(WorkspaceErrors.CreatedFailed);
            }
        }
    }
}
