using Application.Dtos.CommentDto;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.CommentSpecification;
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
    public async Task<Result<PagedResponse<CommentDto>>> GetCommentsByQuestIdAsync(int questId, QueryFilter queryFilter)
    {
        var spec = new GetAllCommentSpecification(questId, queryFilter);
        var countSpec = new CommentCountSpecification(questId);

        var totalRecords = await _commentRepository.CountAsync(countSpec);
        var comments = await _commentRepository.FindAll(spec);

        var commentDtos = mapper.Map<IEnumerable<CommentDto>>(comments);
        var pagedResponse = new PagedResponse<CommentDto>(commentDtos, queryFilter.PageNumber, queryFilter.PageSize, totalRecords);

        return Result<PagedResponse<CommentDto>>.Success(pagedResponse);
    }
    public async Task<Result<CommentDto>> UpdateCommentAsync(UpdateCommentDto updateCommentDto)
    {
        var comment = await _commentRepository.Find(c => c.Id == updateCommentDto.CommentId);

        if (comment == null)
            return Result<CommentDto>.Failure(CommentErrors.NotFound);

        if (comment.UserId != updateCommentDto.UserId)
            return Result<CommentDto>.Failure(CommentErrors.AccessDenied);

        if (DateTime.UtcNow > comment.CreatedAt.AddMinutes(5))
            return Result<CommentDto>.Failure(CommentErrors.EditTimeout);

        mapper.Map(updateCommentDto, comment);
        _commentRepository.Update(comment);
        await unitOfWork.SaveAsync();

        var commentDto = mapper.Map<CommentDto>(comment);
        return Result<CommentDto>.Success(commentDto);
    }
    public async Task<Result<bool>> DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await _commentRepository.Find(c => c.Id == commentId);

        if (comment == null)
            return Result<bool>.Failure(CommentErrors.NotFound);
        if (comment.UserId != userId)
            return Result<bool>.Failure(CommentErrors.AccessDenied);

        _commentRepository.Delete(comment);
        await unitOfWork.SaveAsync();

        return Result<bool>.Success(true);
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
