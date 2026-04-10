using Application.Shared;

namespace Application.ServiceAbstractions
{
    public interface ICategoryService
    {
        Task<Result<bool>> IsCategoryInWorkSpaceAsync(int categoryId, int workspaceId);
    }
}
