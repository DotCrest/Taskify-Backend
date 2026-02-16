namespace Application.ServiceAbstractions
{
    public interface IWorkSpaceService
    {
        Task<bool> IsWorkSpaceExsist(int workSpaceId);
    }
}
