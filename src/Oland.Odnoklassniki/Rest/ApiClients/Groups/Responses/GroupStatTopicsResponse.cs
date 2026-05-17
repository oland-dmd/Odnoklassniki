using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.getStatTopics</c>.
/// </summary>
internal sealed record GroupStatTopicsResponse
{
    /// <summary>Список статистики по топикам текущей страницы.</summary>
    [JsonPropertyName("topics")]
    public ICollection<GroupStatTopicDto>? Topics { get; init; }

    /// <summary>Курсор для перехода к следующей странице.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Признак наличия следующей страницы.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество топиков в запрошенном диапазоне.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}
