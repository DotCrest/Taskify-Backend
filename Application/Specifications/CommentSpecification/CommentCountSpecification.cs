using Domain.Models;

namespace Application.Specifications.CommentSpecification;

public class CommentCountSpecification : BaseSpecification<Comment>
{
    public CommentCountSpecification(int questId) : base(c => c.QuestId == questId)
    {
    }
}
