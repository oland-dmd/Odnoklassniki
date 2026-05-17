using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getList</c>.
/// Содержит массив обсуждений и маркер следующей страницы.
/// </summary>
internal sealed class DiscussionListResponse<T>
{
    /// <summary>Маркер для получения следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Список обсуждений.</summary>
    [JsonPropertyName("discussions")]
    public ICollection<T>? Discussions { get; init; }
}
