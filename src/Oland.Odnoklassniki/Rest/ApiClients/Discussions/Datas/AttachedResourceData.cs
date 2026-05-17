using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

/// <summary>
/// DTO вложенного ресурса (фото, видео и т.п.), возвращаемого методом
/// <c>discussions.getAttachedResources</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record AttachedResourceData : BaseOkDto
{
    /// <summary>Идентификатор вложения.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>Ссылка на автора вложения.</summary>
    [JsonPropertyName("author_ref")]
    public string? AuthorRef { get; init; }

    /// <summary>Количество лайков.</summary>
    [JsonPropertyName("likes_count")]
    public int? LikesCount { get; init; }

    /// <summary>Количество комментариев.</summary>
    [JsonPropertyName("comments_count")]
    public int? CommentsCount { get; init; }

    /// <summary>Длительность медиа в секундах (для видео/аудио).</summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; init; }
}
