using Application.ServiceAbstractions;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class WorkSpaceService(IUnitOfWork<Workspace> unitOfWork) : IWorkSpaceService
    {
        private readonly IGenericRepository<Workspace> repo = unitOfWork.Repo;
        public async Task<bool> IsWorkSpaceExsist(int workSpaceId)
        {

            var workSpace = await repo.GetByIdAsync(workSpaceId);
            if (workSpace == null)
                return false;
            return true;

        }
    }
}
