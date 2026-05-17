using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Video.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>video.subscribe</c>.
/// </summary>
internal sealed record VideoSubscribeResponse
{
    /// <summary>Признак успешной подписки.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }
}
