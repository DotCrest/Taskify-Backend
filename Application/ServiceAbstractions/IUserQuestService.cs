using Application.Dtos.UserQuestDtos;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface IUserQuestService
{
    Task<Result<UserQuestDto>> AssignUserToQuest(UserToQuestDto addUserToQuestDto);
    Task<Result<bool>> UnAssignUserFromQuestAsync(UserToQuestDto unAssignUserDto);
    Task<bool> IsUserAssignedToQuest(string userId, int questId);
}
