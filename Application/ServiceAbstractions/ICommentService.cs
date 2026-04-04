using Application.Dtos.CommentDto;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface ICommentService
{
    Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto);
    Task<Result<IEnumerable<CommentDto>>> GetCommentsByPlanIdAsync(int questId);
    Task<Result<CommentDto>> UpdateCommentAsync(UpdateCommentDto updateCommentDto);
    Task<Result<bool>> DeleteCommentAsync(int commentId, string userId);
}
