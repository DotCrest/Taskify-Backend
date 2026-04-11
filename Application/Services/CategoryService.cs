using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly IGenericRepository<Category> repo = unitOfWork.Repository<Category>();
        public async Task<Result<bool>> IsCategoryInWorkSpaceAsync(int categoryId, int workspaceId)
        {
            var category = await repo.GetByIdAsync(categoryId);
            if (category == null)
                return Result<bool>.Failure(CategoryErrors.NotFound);
            if (category.WorkspaceId != workspaceId)
                return Result<bool>.Failure(CategoryErrors.NotInWorkspace);
            return Result<bool>.Success(true);
        }
    }
}
