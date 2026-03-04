using Application.Dtos.TagDtos;
using Application.Shared;

namespace Application.ServiceAbstractions
{
    public interface ITagService
    {
        Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default);
        Task<Result<TagToReturnDto>> CreateTagAsync(TagDto tagDto, string userId);
    }
}
