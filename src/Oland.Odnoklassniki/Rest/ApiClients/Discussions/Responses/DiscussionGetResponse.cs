using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.get</c>.
/// Основной объект обсуждения вложен в поле <c>discussion</c>.
/// </summary>
internal sealed class DiscussionGetResponse<T>
{
    /// <summary>Объект обсуждения.</summary>
    [JsonPropertyName("discussion")]
    public T? Discussion { get; init; }
}
