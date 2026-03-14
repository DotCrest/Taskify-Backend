using Application.Dtos.QuestDtos;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions;

public interface IQuestService
{
    Task BulkUpdateQuestCategoryAsync(int workspaceId, int? categoryId = (int?)null);
    Task<Result<PagedResponse<QuestToReturnDto>>> GetAllQuests(string userId, int spaceId, QueryFilter queryFilter);
    Task<Result<QuestToReturnDto>> GetQuestByIdAsync(string userId, int questId, int spaceId);
    Task<Result<QuestToReturnDto>> CreateQuestAsync(string userId, QuestToCreateDto createQuestDto, int spaceId);
    Task<Result<bool>> UpdateQuestAsync(string userId, int questId, int spaceId, QuestToUpdateDto updateQuestDto);

}
