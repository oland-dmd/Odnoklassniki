using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getDiscussionCommentsCount</c>.
/// </summary>
internal sealed record DiscussionCommentsCountResponse
{
    /// <summary>Количество комментариев.</summary>
    [JsonPropertyName("commentsCount")]
    public int CommentsCount { get; init; }
}
