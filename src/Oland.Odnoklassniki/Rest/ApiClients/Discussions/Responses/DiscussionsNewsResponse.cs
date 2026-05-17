using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getDiscussionsNews</c>.
/// Новости расположены в поле <c>news</c>.
/// </summary>
internal sealed record DiscussionsNewsResponse
{
    /// <summary>Список новостей обсуждений.</summary>
    [JsonPropertyName("news")]
    public ICollection<DiscussionNewsItemData>? News { get; init; }

    /// <summary>Предпочтительный тип новости.</summary>
    [JsonPropertyName("preferred")]
    public string? Preferred { get; init; }
}
