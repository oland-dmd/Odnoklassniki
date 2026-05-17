using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

/// <summary>
/// DTO комментария, возвращаемого новыми методами API: <c>discussions.getComment</c>
/// и <c>discussions.getDiscussionComments</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record CommentDetailData : BaseOkDto
{
    /// <summary>Идентификатор комментария.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>Ссылка на автора комментария (uid или ref).</summary>
    [JsonPropertyName("author_ref")]
    public string? AuthorRef { get; init; }

    /// <summary>Время создания комментария (Unix мс).</summary>
    [JsonPropertyName("created_ms")]
    public long? CreatedMs { get; init; }

    /// <summary>Текст комментария.</summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>Количество лайков на комментарий.</summary>
    [JsonPropertyName("likes_count")]
    public int? LikesCount { get; init; }

    /// <summary>Количество ответов на комментарий.</summary>
    [JsonPropertyName("comments_count")]
    public int? CommentsCount { get; init; }

    /// <summary>Признак заблокированного комментария.</summary>
    [JsonPropertyName("blocked")]
    public bool? Blocked { get; init; }

    /// <summary>Признак сохранения комментария в закладках.</summary>
    [JsonPropertyName("bookmarked")]
    public bool? Bookmarked { get; init; }
}
