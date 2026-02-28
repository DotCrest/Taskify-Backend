using Application.ServiceAbstractions;
using Application.Specifications.QuestSpecifications;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class QuestService(IUnitOfWork unitOfWork) : IQuestService
{
    private readonly IGenericRepository<Quest> repo = unitOfWork.Repository<Quest>();
    public async Task DetachQuestFromWorkspace(int workspaceId, CancellationToken cancellationToken = default)
    {
        var spec = new QuestsByWorkspaceSpecification(workspaceId);
        await repo.BulkUpdateAsync(
            spec,
            setters => setters.SetProperty(q => q.CategoryId, (int?)null),
            cancellationToken
        );
    }
}
