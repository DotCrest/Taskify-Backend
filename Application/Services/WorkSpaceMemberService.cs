using Application.ServiceAbstractions;
using Application.Specifications.WorkSpaceMemberSpecifications;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class WorkSpaceMemberService(IUnitOfWork unitOfWork) : IWorkSpaceMemberService
{
    private readonly IGenericRepository<WorkspaceMember> workSpaceMemberRepository = unitOfWork.Repository<WorkspaceMember>();
    public async Task<WorkspaceMember?> GetWorkSpaceMemberAsync(int workspaceId, string userId)
    {
        var specification = new WorkSpaceMemberSpecification(workspaceId, userId);
        var workSpaceMember = await workSpaceMemberRepository.Find(specification);
        return workSpaceMember;
    }
    public async Task AddWorkSpaceMemberAsync(WorkspaceMember workspaceMember)
    {
        await workSpaceMemberRepository.AddAsync(workspaceMember);
        await unitOfWork.SaveAsync();
    }

}