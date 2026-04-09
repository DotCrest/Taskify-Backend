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
                              IQuestService questService,
                              IWorkSpaceMemberService workSpaceMemberService) : IUserQuestService
{
    private IGenericRepository<UserQuest> userQuestsRepository = unitOfWork.Repository<UserQuest>();
    public async Task<Result<UserQuestDto>> AssignUserToQuest(UserToQuestDto addUserToQuestDto)
    {
        var validationResult = await ExternalValidationsSteps(addUserToQuestDto);
        if (!validationResult.IsSuccess)
            return Result<UserQuestDto>.Failure(validationResult.ErrorsList);

        var isAssignedToQuest = await IsUserAssignedToQuest(addUserToQuestDto.AssigneeId, addUserToQuestDto.QuestId);
        if (isAssignedToQuest)
            return Result<UserQuestDto>.Failure(UserQuestErrors.UserAlreadyAssigned);

        var userQuest = mapper.Map<UserQuest>(addUserToQuestDto);
        await userQuestsRepository.AddAsync(userQuest);
        await unitOfWork.SaveAsync();

        var recordAfterIncludeUser = await userQuestsRepository.Find(x => x.UserId == userQuest.UserId && x.QuestId == userQuest.QuestId, u => u.User);
        return Result<UserQuestDto>.Success(mapper.Map<UserQuestDto>(recordAfterIncludeUser));
    }
    public async Task<Result<bool>> UnAssignUserFromQuestAsync(string userId, int questId)
    {
        var IsQuestExisted = await questService.IsQuestExisted(questId);
        if (!IsQuestExisted)
            return Result<bool>.Failure(QuestErrors.NotFound);

        var assignedToQuest = await IsUserAssignedToQuest(userId, questId);
        if (!assignedToQuest)
            return Result<bool>.Failure(UserQuestErrors.UserNotAssignedToQuest);

        var userQuest = await userQuestsRepository.Find(uq => uq.UserId == userId && uq.QuestId == questId);
        userQuestsRepository.Delete(userQuest!);
        await unitOfWork.SaveAsync();
        return Result<bool>.Success(true);
    }
    private async Task<Result<bool>> ExternalValidationsSteps(UserToQuestDto userToQuestDto)
    {
        var isQuestExist = await questService.IsQuestExisted(userToQuestDto.QuestId, userToQuestDto.SpaceId);
        if (!isQuestExist)
            return Result<bool>.Failure(QuestErrors.NotFound);

        // for user to be assigned to quest, both assigner and assignee should be in the workspace of the quest

        var isUserInWorkspace = await workSpaceMemberService.IsUserInWorkSpaceAsync(userToQuestDto.WorkspaceId, userToQuestDto.AssigneeId);
        if (!isUserInWorkspace)
            return Result<bool>.Failure(WorkspaceErrors.UserNotInWorkspace);

        var isAssignerInWorkspace = await workSpaceMemberService.IsUserInWorkSpaceAsync(userToQuestDto.WorkspaceId, userToQuestDto.AssignerId!);
        if (!isAssignerInWorkspace)
            return Result<bool>.Failure(WorkspaceErrors.UserNotInWorkspace);

        return Result<bool>.Success(true);
    }
    public async Task<bool> IsUserAssignedToQuest(string userId, int questId)
    {
        return await userQuestsRepository.AnyAsync(x => x.UserId == userId && x.QuestId == questId);
    }
}
