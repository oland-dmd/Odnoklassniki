using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.getCounters</c>.
/// Счётчики возвращаются в поле <c>counters</c>.
/// </summary>
internal sealed record GroupCountersResponse
{
    /// <summary>Объект со счётчиками группы.</summary>
    [JsonPropertyName("counters")]
    public GroupCountersDto? Counters { get; init; }
}
