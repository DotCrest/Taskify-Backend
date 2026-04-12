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

    internal static Faker<UpdateCommentDto> GetUpdateCommentDto(int seed = 0) => new Faker<UpdateCommentDto>()
        .UseSeed(seed)
        .RuleFor(c => c.CommentId, f => f.Random.Int(1, 100))
        .RuleFor(c => c.UserComment, f => f.Lorem.Paragraph(1))
        .RuleFor(c => c.UserId, f => f.Random.Guid().ToString());

    /// <summary>
    /// Creates a Bogus faker for generating fake Comment objects with test data.
    /// 
    /// The seed parameter controls the CreatedAt timestamp generation to support testing of the 5-minute edit timeout
    /// 
    /// When seed % 2 == 0 (even seed):
    ///     - CreatedAt is set to DateTime.UtcNow (current time)
    ///     - This generates a comment that was just created, well within the 5-minute edit window
    /// 
    /// When seed % 2 == 1 (odd seed):
    ///     - CreatedAt is set to a date in the past (f.Date.Past())
    ///     - This generates a comment that was created a long time ago, beyond the 5-minute edit window
    ///  
    /// Usage in tests:
    /// - Use seed % 2 == 0 (e.g., seed: 0, 2, 4): For testing successful comment updates
    /// - Use seed % 2 == 1 (e.g., seed: 1, 3, 5): For testing EditTimeout error cases
    /// </summary>
    internal static Faker<Comment> GetComment(int seed = 0) => new Faker<Comment>()
        .UseSeed(seed)
        .RuleFor(c => c.Id, f => f.Random.Int(1, 100))
        .RuleFor(c => c.Content, f => f.Lorem.Paragraph(1))
        .RuleFor(c => c.UserId, f => f.Random.Guid().ToString())
        .RuleFor(c => c.QuestId, f => f.Random.Int(1, 100))
        .RuleFor(c => c.CreatedAt, f => seed % 2 == 0 ? DateTime.UtcNow : f.Date.Past());
}
