using Application.Dtos.CommentDto;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class CommentService(IUnitOfWork unitOfWork,
                            IMapper mapper,
                            IQuestService questService,
                            IUserQuestService userQuestService) : ICommentService
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.Repository<Comment>();
    public async Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto)
    {
        var externalValidationsResult = await ExternalValidationsSteps(addCommentDto.UserId!, addCommentDto.QuestId);
        if (!externalValidationsResult.IsSuccess)
            return Result<CommentDto>.Failure(externalValidationsResult.ErrorsList);

        var comment = mapper.Map<Comment>(addCommentDto);
        await _commentRepository.AddAsync(comment);
        await unitOfWork.SaveAsync();

        var commentDto = mapper.Map<CommentDto>(comment);
        return Result<CommentDto>.Success(commentDto);
    }
    private async Task<Result<bool>> ExternalValidationsSteps(string userId, int questId)
    {
        var isQuestExist = await questService.IsQuestExisted(questId);
        if (!isQuestExist)
            return Result<bool>.Failure(QuestErrors.NotFound);

        var isUserAssignedToQuest = await userQuestService.IsUserAssignedToQuest(userId, questId);
        if (!isUserAssignedToQuest)
            return Result<bool>.Failure(CommentErrors.AccessDenied);

        return Result<bool>.Success(true);
    }
}
