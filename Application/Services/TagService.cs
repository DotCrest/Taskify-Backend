using Application.ServiceAbstractions;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class TagService(IUnitOfWork unitOfWork) : ITagService
    {
        private readonly IGenericRepository<Tag> repository = unitOfWork.Repository<Tag>();
        public async Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default)
        {
            await repository.BulkDeleteAsync(t => t.WorkspaceId == workSpaceId, cancellationToken);
        }
    }
}
