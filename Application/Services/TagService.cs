using Application.Dtos.TagDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.TagSpecifiacations;
using Application.Specifications.WorkspaceSpecifications;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services
{
    public class TagService(IUnitOfWork unitOfWork,
                            IMapper mapper) : ITagService
    {
        private readonly IGenericRepository<Tag> tagRepo = unitOfWork.Repository<Tag>();
        private readonly IGenericRepository<Workspace> workspaceRepo = unitOfWork.Repository<Workspace>();
        public async Task<Result<TagToReturnDto>> CreateTagAsync(TagDto tagDto, string userId)
        {
            var validation = await CheckWorkspaceExistenceAndUserAccessAsync(tagDto.WorkspaceId, userId);
            if (!validation.IsSuccess)
                return Result<TagToReturnDto>.Failure(validation.ErrorsList);

            var existedTag = await CheckIfTagAlreadyExists(tagDto.Name, tagDto.WorkspaceId);
            if (existedTag is not null)
                return Result<TagToReturnDto>.Failure(TagErrors.AlreadyExisted);

            var tag = mapper.Map<Tag>(tagDto);
            await tagRepo.AddAsync(tag);
            await unitOfWork.SaveAsync();
            var tagToReturnDto = mapper.Map<TagToReturnDto>(tag);
            return Result<TagToReturnDto>.Success(tagToReturnDto);
        }
        public async Task<Result<PagedResponse<TagToReturnDto>>> GetAllTagsAsync(QueryFilter queryFilter, int workspaceId, string userId)
        {
            var validation = await CheckWorkspaceExistenceAndUserAccessAsync(workspaceId, userId);
            if (!validation.IsSuccess)
                return Result<PagedResponse<TagToReturnDto>>.Failure(validation.ErrorsList);

            var countspecification = new TagByWorkspaceCountSpecification(workspaceId);
            var tagsCount = await tagRepo.CountAsync(countspecification);

            var specification = new TagByWorkspaceSpecification(queryFilter, workspaceId);
            var tags = await tagRepo.FindAll(specification);
            var tagsToReturnDto = mapper.Map<IEnumerable<TagToReturnDto>>(tags);

            var pagedResponse = new PagedResponse<TagToReturnDto>(tagsToReturnDto, queryFilter.PageNumber, queryFilter.PageSize, tagsCount);
            return Result<PagedResponse<TagToReturnDto>>.Success(pagedResponse);
        }
        public async Task<Result<TagToReturnDto>> GetTagByIdAsync(int tagId, string userId)
        {
            var tag = await tagRepo.GetByIdAsync(tagId);
            if (tag == null)
                return Result<TagToReturnDto>.Failure(TagErrors.NotFound);

            var validation = await CheckWorkspaceExistenceAndUserAccessAsync(tag.WorkspaceId, userId);
            if (!validation.IsSuccess)
                return Result<TagToReturnDto>.Failure(validation.ErrorsList);

            var tagToReturn = mapper.Map<TagToReturnDto>(tag);
            return Result<TagToReturnDto>.Success(tagToReturn);
        }
        public async Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default)
        {
            await tagRepo.BulkDeleteAsync(t => t.WorkspaceId == workSpaceId, cancellationToken);
        }
        private async Task<Tag?> CheckIfTagAlreadyExists(string tagName, int workspaceId)
        {
            var tagSpecification = new TagByNameAndWorkspaceSpecification(tagName, workspaceId);
            return await tagRepo.Find(tagSpecification);
        }
        private async Task<Result<bool>> CheckWorkspaceExistenceAndUserAccessAsync(int workspaceId, string userId)
        {
            var workspaceSpecification = new WorkspaceGetByIdSpecification(workspaceId);
            var workspace = await workspaceRepo.Find(workspaceSpecification);
            if (workspace == null)
                return Result<bool>.Failure(WorkspaceErrors.NotFound);
            var isMember = workspace.WorkspaceMembers.Any(x => x.UserId == userId);
            if (!isMember)
                return Result<bool>.Failure(TagErrors.AccessDenied);
            return Result<bool>.Success(true);
        }
    }
}
