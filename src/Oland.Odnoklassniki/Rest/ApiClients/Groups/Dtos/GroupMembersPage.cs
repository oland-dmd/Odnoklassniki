namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>A single lossless roster page. Missing protocol fields never mean an empty group.</summary>
public sealed record GroupMembersPage(
    IReadOnlyList<GroupMemberDto>? Members, string? Anchor, bool? HasMore, int? TotalCount)
{
    public bool? MembersHidden { get; init; }
    public bool IsValid => Members is not null && HasMore.HasValue && TotalCount is >= 0;
}
