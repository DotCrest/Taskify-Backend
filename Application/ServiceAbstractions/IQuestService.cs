using Application.Dtos.QuestDtos;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions;

public interface IQuestService
{
    Task BulkUpdateQuestCategoryAsync(int workspaceId, int? categoryId = (int?)null);
    Task<Result<PagedResponse<QuestToReturnDto>>> GetAllQuests(string userId, int spaceId, QueryFilter queryFilter);
}
