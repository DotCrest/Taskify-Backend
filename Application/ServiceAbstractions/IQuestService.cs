namespace Application.ServiceAbstractions;

public interface IQuestService
{
    Task BulkUpdateQuestCategoryAsync(int workspaceId, int? categoryId = (int?)null);
}
