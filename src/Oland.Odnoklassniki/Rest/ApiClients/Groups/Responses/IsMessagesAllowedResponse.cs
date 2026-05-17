using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.isMessagesAllowed</c>.
/// </summary>
internal sealed record IsMessagesAllowedResponse
{
    /// <summary>
    /// <see langword="true"/>, если сообщения от группы разрешены текущему пользователю;
    /// <see langword="false"/> — если пользователь отписался от сообщений группы.
    /// </summary>
    [JsonPropertyName("allowed")]
    public bool Allowed { get; init; }
}
