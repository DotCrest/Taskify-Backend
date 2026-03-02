using Domain.Models;

namespace Application.ServiceAbstractions
{
    public interface IWorkSpaceMemberService
    {
        Task<WorkspaceMember?> GetWorkSpaceMemberAsync(int workspaceId, string userId);
        Task AddWorkSpaceMemberAsync(WorkspaceMember workspaceMember);
    }
}