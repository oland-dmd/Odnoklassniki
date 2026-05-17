using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.pinGroupFeed</c>.
/// </summary>
internal sealed record PinGroupFeedResponse
{
    /// <summary><see langword="true"/>, если запись успешно закреплена в ленте группы.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }
}
