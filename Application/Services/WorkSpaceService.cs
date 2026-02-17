using Application.ServiceAbstractions;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class WorkSpaceService(IUnitOfWork unitOfWork) : IWorkSpaceService
    {
        private readonly IGenericRepository<Workspace> repo = unitOfWork.Repository<Workspace>();
        public async Task<Workspace?> GetWorkSpaceById(int workSpaceId)
        {
            var workspace = await repo.GetByIdAsync(workSpaceId);
            return workspace;
        }
    }
}
