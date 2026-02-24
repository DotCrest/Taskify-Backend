using Application.Dtos.WorkspaceDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Pagination;
using Application.Specifications.WorkspaceSpecifications;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class WorkSpaceService(IUnitOfWork unitOfWork,
        IMapper mapper) : IWorkSpaceService
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
    }
}
