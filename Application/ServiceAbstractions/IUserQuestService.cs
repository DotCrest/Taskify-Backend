using Application.Dtos.UserQuestDtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface IUserQuestService
{
    Task<Result<UserQuestDto>> AssignUserToQuest(AddUserToQuestDto addUserToQuestDto);
    Task<bool> IsUserAssignedToQuest(string userId, int questId);
}
