using Application.Dtos.CommentDto;
using Application.Shared.Pagination;
using Bogus;
using Domain.Models;

namespace Application.Tests.Fakers;

internal class CommentFaker
{
    internal static Faker<QueryFilter> GetFakeQueryFilter() => new Faker<QueryFilter>()
        .RuleFor(q => q.PageNumber, f => f.Random.Int(1, 10))
        .RuleFor(q => q.PageSize, f => f.Random.Int(5, 20));

    internal static Faker<AddCommentDto> GetAddCommentDto() => new Faker<AddCommentDto>()
        .RuleFor(c => c.UserComment, f => f.Lorem.Paragraph(1))
        .RuleFor(c => c.QuestId, f => f.Random.Int(1, 100))
        .RuleFor(c => c.UserId, f => f.Random.Guid().ToString());

    internal static Faker<UpdateCommentDto> GetUpdateCommentDto() => new Faker<UpdateCommentDto>()
        .RuleFor(c => c.UserComment, f => f.Lorem.Paragraph(1))
        .RuleFor(c => c.CommentId, f => f.Random.Int(1, 100))
        .RuleFor(c => c.UserId, f => f.Random.Guid().ToString());

    internal static Faker<Comment> GetComment() => new Faker<Comment>()
        .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.Content, f => f.Lorem.Paragraph(1))
        .RuleFor(c => c.QuestId, f => f.Random.Int(1, 100))
        .RuleFor(c => c.UserId, f => f.Random.Guid().ToString())
        .RuleFor(c => c.CreatedAt, f => f.Date.Recent());
}
