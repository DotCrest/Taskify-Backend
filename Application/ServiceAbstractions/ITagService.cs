namespace Application.ServiceAbstractions
{
    public interface ITagService
    {
        Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default);
    }
}
