using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ устаревшего метода <c>discussions.getDiscussions</c>.
/// </summary>
internal sealed record DiscussionsLegacyResponse
{
    [JsonPropertyName("discussions")]
    public ICollection<DiscussionsLegacyItem>? Discussions { get; init; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}

internal sealed record DiscussionsLegacyItem
{
    [JsonPropertyName("entityId")]
    public string? EntityId { get; init; }

    [JsonPropertyName("entityType")]
    public string? EntityType { get; init; }

    [JsonPropertyName("newCommentsCount")]
    public short? NewCommentsCount { get; init; }

    [JsonPropertyName("subjectLabel")]
    public string? SubjectLabel { get; init; }
}
