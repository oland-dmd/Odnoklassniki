using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.getStatTopic</c>.
/// Статистика одного топика возвращается в поле <c>topic</c>.
/// </summary>
internal sealed record GroupStatTopicResponse
{
    /// <summary>Статистика запрошенного топика.</summary>
    [JsonPropertyName("topic")]
    public GroupStatTopicDto? Topic { get; init; }
}
