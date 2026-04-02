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

        var isAssignedToQuest = await IsUserAssignedToQuest(addUserToQuestDto.UserId, addUserToQuestDto.QuestId);
        if (isAssignedToQuest)
            return Result<UserQuestDto>.Failure(UserQuestErrors.UserAlreadyAssigned);

        var userQuest = mapper.Map<UserQuest>(addUserToQuestDto);
        await userQuestsRepository.AddAsync(userQuest);
        await unitOfWork.SaveAsync();

        var recordAfterIncludeUser = await userQuestsRepository.Find(x => x.UserId == userQuest.UserId && x.QuestId == userQuest.QuestId, u => u.User);
        return Result<UserQuestDto>.Success(mapper.Map<UserQuestDto>(recordAfterIncludeUser));
    }
    public async Task<Result<bool>> UnAssignUserFromQuestAsync(UserToQuestDto unAssignUserDto)
    {
        var externalValidationResult = await ExternalValidationsSteps(unAssignUserDto);
        if (!externalValidationResult.IsSuccess)
            return Result<bool>.Failure(externalValidationResult.ErrorsList);

        var assignedToQuest = await IsUserAssignedToQuest(unAssignUserDto.UserId, unAssignUserDto.QuestId);
        if (!assignedToQuest)
            return Result<bool>.Failure(UserQuestErrors.UserNotAssignedToQuest);

        var userQuest = await userQuestsRepository.Find(uq => uq.UserId == unAssignUserDto.UserId && uq.QuestId == unAssignUserDto.QuestId);
        userQuestsRepository.Delete(userQuest!);
        await unitOfWork.SaveAsync();
        return Result<bool>.Success(true);
    }
    private async Task<Result<bool>> ExternalValidationsSteps(UserToQuestDto userToQuestDto)
    {
        var isQuestExist = await questService.IsQuestExisted(userToQuestDto.QuestId);
        if (!isQuestExist)
            return Result<bool>.Failure(QuestErrors.NotFound);

        var isUserInWorkspace = await workSpaceMemberService.IsUserInWorkSpaceAsync(userToQuestDto.WorkspaceId, userToQuestDto.UserId);
        if (!isUserInWorkspace)
            return Result<bool>.Failure(WorkspaceErrors.UserNotInWorkspace);

        return Result<bool>.Success(true);
    }
    public async Task<bool> IsUserAssignedToQuest(string userId, int questId)
    {
        return await userQuestsRepository.AnyAsync(x => x.UserId == userId && x.QuestId == questId);
    }
}
