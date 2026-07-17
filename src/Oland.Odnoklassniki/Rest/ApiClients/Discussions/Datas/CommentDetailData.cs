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

    /// <summary>
    /// Время создания комментария (Unix мс). Для некоторых типов обсуждений (замечено на личных
    /// <c>USER_STATUS</c>/<c>USER_PHOTO</c>) OK не отдаёт это поле вовсе (<see langword="null"/>) — тогда
    /// используется резервный <see cref="Date"/>, см. <see cref="TimestampMs"/>.
    /// </summary>
    [JsonPropertyName("created_ms")]
    public long? CreatedMs { get; init; }

    /// <summary>
    /// Резервное человекочитаемое время создания ("yyyy-MM-dd HH:mm:ss", московское время) — приходит
    /// вместо <see cref="CreatedMs"/> для обсуждений, где числовое поле не отдаётся API.
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; init; }

    /// <summary>Итоговое время создания в Unix мс: <see cref="CreatedMs"/>, а при его отсутствии — разбор <see cref="Date"/>.</summary>
    public long TimestampMs => CreatedMs ?? OkLegacyDateParser.ParseToUnixMs(Date);

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
