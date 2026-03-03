using Application.ServiceAbstractions;
using Application.Specifications.CategorySpecifications;
using Application.Specifications.QuestSpecifications;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class QuestService(IUnitOfWork unitOfWork) : IQuestService
{
    private readonly IGenericRepository<Quest> questRepo = unitOfWork.Repository<Quest>();
    private readonly IGenericRepository<Category> categoryRepo = unitOfWork.Repository<Category>();
    public async Task BulkUpdateQuestCategoryAsync(int workspaceId, int? categoryId = (int?)null)
    {
        var categorySpec = new CategoryByWorkspaceSpecification(workspaceId);
        var categories = await categoryRepo.FindAll(categorySpec);
        var categoryIds = categories.Select(c => c.Id).ToList();

        var questSpec = new QuestsByCategorySpecification(categoryIds);
        await questRepo.BulkUpdateAsync(
            questSpec,
            setters => setters.SetProperty(q => q.CategoryId, categoryId)
        );
    }
}
