using Domain.Models;

namespace Application.ServiceAbstractions
{
    public interface IWorkSpaceService
    {
        Task<Workspace?> GetWorkSpaceById(int workSpaceId);
    }
}
