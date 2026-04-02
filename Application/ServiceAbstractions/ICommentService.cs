using Application.Dtos.CommentDto;
using Application.Shared;

namespace Application.ServiceAbstractions;

public interface ICommentService
{
    Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto);

}
