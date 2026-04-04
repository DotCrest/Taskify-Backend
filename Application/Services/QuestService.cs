using Application.Dtos.QuestDtos;
using Application.ServiceAbstractions;
using Application.Shared;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using Application.Specifications.CategorySpecifications;
using Application.Specifications.QuestSpecifications;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;

namespace Application.Services;

public class QuestService(IUnitOfWork unitOfWork,
                          ISpaceService spaceService,
                          IWorkSpaceMemberService workSpaceMemberService,
                          IMapper mapper,
                          ICategoryService categoryService) : IQuestService
{
    private readonly IGenericRepository<Quest> questRepo = unitOfWork.Repository<Quest>();
    private readonly IGenericRepository<Category> categoryRepo = unitOfWork.Repository<Category>();


    public async Task<Result<PagedResponse<QuestToReturnDto>>> GetAllQuests(string userId, int spaceId, QueryFilter queryFilter)
    {
        var space = await spaceService.GetSpaceAsync(spaceId);
        if (!space.IsSuccess)
            return Result<PagedResponse<QuestToReturnDto>>.Failure(SpaceErrors.NotFound);
        var isMember = await IsUserMemberOfWorkspace(userId, space.Value!.WorkspaceId);
        if (isMember == false)
            return Result<PagedResponse<QuestToReturnDto>>.Failure(WorkspaceErrors.AccessDenied);
        var questspec = new GetAllQuestSpecification(queryFilter, spaceId);
        var countspec = new QuestCountSpecification(spaceId);
        var totalRecords = await questRepo.CountAsync(countspec);
        var quests = await questRepo.FindAll(questspec);
        var data = mapper.Map<IEnumerable<QuestToReturnDto>>(quests);
        var pagedResponse = new PagedResponse<QuestToReturnDto>(data, queryFilter.PageNumber, queryFilter.PageSize, totalRecords);
        return Result<PagedResponse<QuestToReturnDto>>.Success(pagedResponse);

    }

    public async Task<Result<QuestToReturnDto>> GetQuestByIdAsync(string userId, int questId, int spaceId)
    {
        var space = await spaceService.GetSpaceAsync(spaceId);
        if (!space.IsSuccess)
            return Result<QuestToReturnDto>.Failure(SpaceErrors.NotFound);
        var isMember = await IsUserMemberOfWorkspace(userId, space.Value!.WorkspaceId);
        if (isMember == false)
            return Result<QuestToReturnDto>.Failure(WorkspaceErrors.AccessDenied);
        var spec = new GetQuestByIdSpecification(questId, spaceId);
        var quest = await questRepo.Find(spec);
        if (quest == null)
            return Result<QuestToReturnDto>.Failure(QuestErrors.NotFound);
        var questToReturn = mapper.Map<QuestToReturnDto>(quest);
        return Result<QuestToReturnDto>.Success(questToReturn);

    }

    public async Task<Result<QuestToReturnDto>> CreateQuestAsync(string userId, QuestToCreateDto createQuestDto, int spaceId)
    {
        var space = await spaceService.GetSpaceAsync(spaceId);
        if (!space.IsSuccess)
            return Result<QuestToReturnDto>.Failure(SpaceErrors.NotFound);
        if (createQuestDto.CategoryId != null)
        {
            var isMember = await categoryService.IsCategoryInWorkSpaceAsync(createQuestDto.CategoryId.Value, space.Value!.WorkspaceId);
            if (!isMember.IsSuccess)
                return Result<QuestToReturnDto>.Failure(WorkspaceErrors.AccessDenied);
        }
        var Quest = new Quest()
        {
            Title = createQuestDto.Title,
            Description = createQuestDto.Description,
            CreatedAt = DateTime.UtcNow,
            SpaceId = spaceId,
            CategoryId = createQuestDto.CategoryId,
            AuthorId = userId,
            Status = (QuestStatusEnum)createQuestDto.Status,
            Priority = (PriorityEnum)createQuestDto.Priority,
        };
        await questRepo.AddAsync(Quest);
        var isCreated = await unitOfWork.SaveAsync();
        if (isCreated <= 0)
            return Result<QuestToReturnDto>.Failure(QuestErrors.CreatedFailed);
        var questToReturn = mapper.Map<QuestToReturnDto>(Quest);
        return Result<QuestToReturnDto>.Success(questToReturn);
    }
    public async Task<Result<bool>> UpdateQuestAsync(string userId, int questId, int spaceId, QuestToUpdateDto updateQuestDto)
    {
        var quest = await questRepo.GetByIdAsync(questId);
        if (quest is null || quest.SpaceId != spaceId)
            return Result<bool>.Failure(QuestErrors.NotFound);
        var space = await spaceService.GetSpaceAsync(spaceId);
        if (!space.IsSuccess)
            return Result<bool>.Failure(SpaceErrors.NotFound);
        if (updateQuestDto.CategoryId != null && updateQuestDto.CategoryId != quest.CategoryId)
        {
            var isCategoryValid = await categoryService.IsCategoryInWorkSpaceAsync(updateQuestDto.CategoryId.Value, space.Value!.WorkspaceId);
            if (!isCategoryValid.IsSuccess)
                return Result<bool>.Failure(WorkspaceErrors.AccessDenied);
        }
        quest.Title = updateQuestDto.Title;
        quest.Description = updateQuestDto.Description;
        quest.CategoryId = updateQuestDto.CategoryId;
        quest.Status = (QuestStatusEnum)updateQuestDto.Status;
        quest.Priority = (PriorityEnum)updateQuestDto.Priority;
        questRepo.Update(quest);

        var result = await unitOfWork.SaveAsync() > 0;

        return result
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(QuestErrors.UpdatedFailed);
    }
    public async Task<Result<bool>> DeleteQuestAsync(string userId, int questId, int spaceId)
    {
        var quest = await questRepo.GetByIdAsync(questId);
        if (quest is null || quest.SpaceId != spaceId)
            return Result<bool>.Failure(QuestErrors.NotFound);
        if (quest.AuthorId != userId)
            return Result<bool>.Failure(WorkspaceErrors.AccessDenied);
        questRepo.Delete(quest);
        var result = await unitOfWork.SaveAsync() > 0;
        return result
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(QuestErrors.DeleteFailed);

    }
    public async Task BulkUpdateQuestCategoryAsync(int workspaceId, int? categoryId = (int?)null)
    {
        var categorySpec = new CategoryByWorkspaceSpecification(workspaceId);
        var categories = await categoryRepo.FindAll(categorySpec);
        var categoryIds = categories.Select(c => c.Id).ToList();

        var questSpec = new QuestsByCategorySpecification(categoryIds);
        await questRepo.BulkUpdateAsync(
            questSpec,
            setters => setters.SetProperty(q => q.CategoryId, categoryId)
        );
    }
    private async Task<bool> IsUserMemberOfWorkspace(string userId, int workspaceId)
    {
        var workSpaceMember = await workSpaceMemberService.GetWorkSpaceMemberAsync(workspaceId, userId);
        if (workSpaceMember != null)
            return true;
        return false;

    }
    public async Task<bool> IsQuestExisted(int questId)
    {
        return await questRepo.AnyAsync(q => q.Id == questId);
    }

    public async Task<bool> IsQuestExisted(int questId, int spaceId)
    {
        return await questRepo.AnyAsync(q => q.Id == questId && q.SpaceId == spaceId);
    }
}
