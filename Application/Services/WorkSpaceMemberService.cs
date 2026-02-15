using Application.ServiceAbstractions;
using Application.Specifications.WorkSpaceMemberSpecifications;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class WorkSpaceMemberService(IGenericRepository<WorkspaceMember> workSpaceMemberRepository) : IWorkSpaceMemberService
{
    public async Task<WorkspaceMember?> GetWorkSpaceMemberAsync(int workspaceId, string userId)
    {
        var specification = new WorkSpaceMemberSpecification(workspaceId, userId);
        var workSpaceMember = await workSpaceMemberRepository.Find(specification);
        return workSpaceMember;
    }
}
