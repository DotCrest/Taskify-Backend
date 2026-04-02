using Application.Dtos.CommentDto;
using Application.ServiceAbstractions;
using Application.Shared;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class CommentService(IUnitOfWork unitOfWork,
                            IMapper mapper) : ICommentService
{
    private readonly IGenericRepository<Comment> _commentRepository = unitOfWork.Repository<Comment>();
    public async Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto)
    {
        // TODO: validate quest existince 
        // TODO: check if user is assigned to this quest
        var comment = mapper.Map<Comment>(addCommentDto);
        await _commentRepository.AddAsync(comment);
        await unitOfWork.SaveAsync();
        var commentDto = mapper.Map<CommentDto>(comment);
        return Result<CommentDto>.Success(commentDto);
    }
}
