using Application.Dtos.InvitationDtos;
using Application.Shared.Pagination;
using Bogus;
using Domain.Models;

namespace Application.Tests.Fakers;

internal class InvitationFaker
{
    internal static readonly List<InvitationStatusEnum> statusEnums = Enum.GetValues<InvitationStatusEnum>().ToList();

    internal static Faker<QueryFilter> GetFakeQueryFilter() => new Faker<QueryFilter>()
        .RuleFor(q => q.PageNumber, f => f.Random.Int(1, 10))
        .RuleFor(q => q.PageSize, f => f.Random.Int(5, 20));

    /// <summary>
    /// Creates a Bogus faker for generating fake GetInvitationDto objects with test data.
    /// </summary>
    /// <param name="index">Index used to select status from the available enum values (via modulo); only applied when seed is even</param>
    /// <param name="seed">Seed value used for deterministic randomization; controls whether Status property is populated or null</param>
    /// <returns>A configured Faker&lt;GetInvitationDto&gt; instance for generating test DTO objects</returns>
    internal static Faker<GetInvitationDto> GetFakeGetInvitationDto(int index, int seed = 0) => new Faker<GetInvitationDto>()
        .RuleFor(i => i.WorkspaceId, f => f.Random.Int(1, 100))
        .RuleFor(i => i.Status, f => seed % 2 == 0 ? statusEnums[index % statusEnums.Count].ToString() : null)
        .RuleFor(i => i.UserId, f => f.Random.Guid().ToString());

    /// <summary>
    /// Creates a Bogus faker for generating fake Invitation objects with test data.
    /// </summary>
    /// <param name="index">Index used to select status from the available enum values (via modulo)</param>
    /// <param name="seed">Seed value used for deterministic randomization; also controls status and date generation logic</param>
    /// <returns>A configured Faker&lt;Invitation&gt; instance for generating test invitation objects</returns>
    internal static Faker<Invitation> GetFakeInvitation(int index, int seed = 0) => new Faker<Invitation>()
        .UseSeed(seed)
        .RuleFor(i => i.Id, f => f.Random.Int(1, 1000))
        .RuleFor(i => i.ReceiverEmail, f => f.Internet.Email())
        .RuleFor(i => i.Status, f => seed % 2 == 0 ?
                statusEnums[index % statusEnums.Count] :
                statusEnums[f.IndexFaker % statusEnums.Count])
        .RuleFor(i => i.ReceiverRole, f => f.PickRandom("Admin", "Member", "Viewer"))
        .RuleFor(i => i.Token, f => f.Random.Guid().ToString())
        .RuleFor(i => i.CreatedAt, f =>
                seed % 2 == 0 ?
                f.Date.Between(DateTime.UtcNow.AddDays(-4), DateTime.UtcNow) :
                f.Date.Past())
        .RuleFor(i => i.SenderId, f => f.Random.Guid().ToString())
        .RuleFor(i => i.WorkspaceId, f => f.Random.Int(1, 100));

}
