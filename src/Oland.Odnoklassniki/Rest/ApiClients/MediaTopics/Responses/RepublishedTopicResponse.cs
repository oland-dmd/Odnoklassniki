using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>mediatopic.getRepublishedTopic</c>.
/// </summary>
internal sealed record RepublishedTopicResponse
{
    /// <summary>Идентификатор топика после перепубликации (после модерации или отложенной публикации).</summary>
    [JsonPropertyName("topicId")]
    public string? TopicId { get; init; }
}
