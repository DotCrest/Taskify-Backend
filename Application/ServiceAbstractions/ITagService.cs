using Application.Dtos.TagDtos;
using Application.Shared;
using Application.Shared.Pagination;

namespace Application.ServiceAbstractions
{
    public interface ITagService
    {
        Task<Result<TagToReturnDto>> CreateTagAsync(TagDto tagDto, string userId);
        Task<Result<PagedResponse<TagToReturnDto>>> GetAllTagsAsync(QueryFilter queryFilter, int workspaceId, string userId);
        Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default);

    }
}
