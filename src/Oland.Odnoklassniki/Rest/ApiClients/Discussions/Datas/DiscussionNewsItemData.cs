using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

/// <summary>
/// DTO элемента новостей обсуждений, возвращаемого методом <c>discussions.getDiscussionsNews</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record DiscussionNewsItemData : BaseOkDto
{
    /// <summary>Тип новости.</summary>
    [JsonPropertyName("news_type")]
    public string? NewsType { get; init; }

    /// <summary>Признак наличия новых элементов данного типа.</summary>
    [JsonPropertyName("has_news")]
    public bool? HasNews { get; init; }

    /// <summary>Признак того, что элемент является новым с момента последнего просмотра.</summary>
    [JsonPropertyName("is_new")]
    public bool? IsNew { get; init; }

    /// <summary>Заголовок новости.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>Ссылка для перехода к новости.</summary>
    [JsonPropertyName("link")]
    public string? Link { get; init; }

    /// <summary>Фильтр, к которому относится новость.</summary>
    [JsonPropertyName("filter")]
    public string? Filter { get; init; }
}
