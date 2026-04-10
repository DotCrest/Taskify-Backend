using Application.Dtos.CommentDto;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions;

public interface ICommentService
{
    Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto);
    Task<Result<PagedResponse<CommentDto>>> GetCommentsByQuestIdAsync(int questId, QueryFilter queryFilter);
    Task<Result<CommentDto>> UpdateCommentAsync(UpdateCommentDto updateCommentDto);
    Task<Result<bool>> DeleteCommentAsync(int commentId, string userId);
}
