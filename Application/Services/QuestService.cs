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

public class QuestService(IUnitOfWork unitOfWork, ISpaceService spaceService, IWorkSpaceMemberService workSpaceMemberService, IMapper mapper) : IQuestService
{
    private readonly IGenericRepository<Quest> questRepo = unitOfWork.Repository<Quest>();
    private readonly IGenericRepository<Category> categoryRepo = unitOfWork.Repository<Category>();
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

    public async Task<Result<PagedResponse<QuestToReturnDto>>> GetAllQuests(string userId, int spaceId, QueryFilter queryFilter)
    {
        var space = await spaceService.GetSpaceByIdAsync(spaceId, userId);
        if (space == null)
            return Result<PagedResponse<QuestToReturnDto>>.Failure(SpaceErrors.NotFound);
        var isMember = await IsUserMemberOfWorkspace(userId, space.Value.WorkspaceId);
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
    private async Task<bool> IsUserMemberOfWorkspace(string userId, int workspaceId)
    {
        var workSpaceMember = await workSpaceMemberService.GetWorkSpaceMemberAsync(workspaceId, userId);
        if (workSpaceMember != null)
            return true;
        return false;

    }
}
