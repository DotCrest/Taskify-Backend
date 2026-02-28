namespace Application.ServiceAbstractions;

public interface IQuestService
{
    Task DetachQuestFromWorkspace(int workspaceId, CancellationToken cancellationToken = default);
}
