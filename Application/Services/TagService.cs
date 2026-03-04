using Application.Dtos.TagDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
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
            var workspaceSpecification = new WorkspaceGetByIdSpecification(tagDto.WorkspaceId);
            var workspace = await workspaceRepo.Find(workspaceSpecification);
            if (workspace == null)
                return Result<TagToReturnDto>.Failure(WorkspaceErrors.NotFound);
            var isMember = workspace.WorkspaceMembers.Any(x => x.UserId == userId);
            if (!isMember)
                return Result<TagToReturnDto>.Failure(TagErrors.AccessDenied);
            var tag = mapper.Map<Tag>(tagDto);
            await tagRepo.AddAsync(tag);
            await unitOfWork.SaveAsync();
            var tagToReturnDto = mapper.Map<TagToReturnDto>(tag);
            return Result<TagToReturnDto>.Success(tagToReturnDto);
        }

        public async Task DeleteAllTagsRelatedToWorkspace(int workSpaceId, CancellationToken cancellationToken = default)
        {
            await tagRepo.BulkDeleteAsync(t => t.WorkspaceId == workSpaceId, cancellationToken);
        }
    }
}
