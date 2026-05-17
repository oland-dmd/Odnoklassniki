using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Stream.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>stream.isSubscribed</c>.
/// </summary>
internal sealed record IsSubscribedResponse
{
    /// <summary>Флаг наличия подписки текущего пользователя на ленту указанного владельца.</summary>
    [JsonPropertyName("subscribed")]
    public bool Subscribed { get; init; }
}
