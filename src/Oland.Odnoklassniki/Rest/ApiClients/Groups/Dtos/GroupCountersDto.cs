using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO счётчиков группы. Используется в методе <c>group.getCounters</c>.
/// Содержит часто запрашиваемые счётчики; нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record GroupCountersDto : BaseOkDto
{
    /// <summary>Количество участников группы.</summary>
    [JsonPropertyName("members")]
    public long? Members { get; init; }

    /// <summary>Количество фотографий в группе.</summary>
    [JsonPropertyName("photos")]
    public long? Photos { get; init; }

    /// <summary>Количество фотоальбомов в группе.</summary>
    [JsonPropertyName("photo_albums")]
    public long? PhotoAlbums { get; init; }

    /// <summary>Количество видеозаписей в группе.</summary>
    [JsonPropertyName("videos")]
    public long? Videos { get; init; }

    /// <summary>Количество товаров в группе.</summary>
    [JsonPropertyName("products")]
    public long? Products { get; init; }

    /// <summary>Количество тем оформления (каталогов) в группе.</summary>
    [JsonPropertyName("themes")]
    public long? Themes { get; init; }

    /// <summary>Количество модераторов группы.</summary>
    [JsonPropertyName("moderators")]
    public long? Moderators { get; init; }

    /// <summary>Количество заявок на вступление в группу.</summary>
    [JsonPropertyName("join_requests")]
    public long? JoinRequests { get; init; }

    /// <summary>Количество заблокированных участников.</summary>
    [JsonPropertyName("black_list")]
    public long? BlackList { get; init; }

    /// <summary>Количество закреплённых тем.</summary>
    [JsonPropertyName("pinned_topics")]
    public long? PinnedTopics { get; init; }
}
