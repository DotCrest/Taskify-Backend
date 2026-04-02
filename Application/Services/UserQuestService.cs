using Application.Dtos.UserQuestDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class UserQuestService(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              IQuestService questService) : IUserQuestService
{
    private IGenericRepository<UserQuest> genericRepository = unitOfWork.Repository<UserQuest>();
    public async Task<Result<UserQuestDto>> AssignUserToQuest(AddUserToQuestDto addUserToQuestDto)
    {
        var IsUserAssigned = await IsUserAssignedToQuest(addUserToQuestDto.UserId, addUserToQuestDto.QuestId);
        if (IsUserAssigned)
            return Result<UserQuestDto>.Failure(UserQuestErrors.UserAlreadyAssigned);

        var IsQuestExist = await questService.IsQuestExisted(addUserToQuestDto.QuestId);
        if (!IsQuestExist)
            return Result<UserQuestDto>.Failure(QuestErrors.NotFound);

        // TODO: check if the user exists, and return appropriate errors if not.

        var userQuest = new UserQuest
        {
            UserId = addUserToQuestDto.UserId,
            QuestId = addUserToQuestDto.QuestId
        };

        await genericRepository.AddAsync(userQuest);
        var RecordAfterIncludeUser = await genericRepository.Find(x => x.UserId == userQuest.UserId && x.QuestId == userQuest.QuestId, u => u.User);
        return Result<UserQuestDto>.Success(mapper.Map<UserQuestDto>(RecordAfterIncludeUser));
    }
    public async Task<bool> IsUserAssignedToQuest(string userId, int questId)
    {
        return await genericRepository.AnyAsync(x => x.UserId == userId && x.QuestId == questId);
    }

}
