using Application.Shared.Pagination;
using Domain.Models;

namespace Application.Specifications.CommentSpecification;

public class GetAllCommentSpecification : BaseSpecification<Comment>
{
    public GetAllCommentSpecification(int questId, QueryFilter queryFilter) : base(c => c.QuestId == questId)
    {
        ApplyPagination(queryFilter.PageSize, queryFilter.PageNumber);
    }
}
