using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Представляет комментарий, полученный из API социальной сети «Одноклассники» (OK.ru).
/// Используется при десериализации JSON-ответов от методов вроде <c>mediatopic.getComments</c>
/// или <c>discussions.getComments</c>.
/// </summary>
internal record Comment
{
    /// <summary>
    /// Уникальный идентификатор комментария в системе Одноклассников.
    /// Соответствует полю <c>"id"</c> в JSON-ответе API.
    /// </summary>
    [JsonPropertyName("id")]
    public string ID { get; init; }

    /// <summary>
    /// Идентификатор автора комментария — ID пользователя в Одноклассниках.
    /// Соответствует полю <c>"author_id"</c> в JSON-ответе.
    /// </summary>
    [JsonPropertyName("author_id")]
    public string AuthorId { get; init; }

    /// <summary>
    /// Полное имя автора комментария (например, «Анна Смирнова»).
    /// Получается из профиля пользователя и возвращается API в поле <c>"author_name"</c>.
    /// </summary>
    [JsonPropertyName("author_name")]
    public string AuthorName { get; init; }

    /// <summary>
    /// Короткая ссылка или упоминание автора (например, «ok.ru/anna_smirnova»).
    /// Используется для формирования гиперссылок в интерфейсе.
    /// Соответствует полю <c>"author_ref"</c> в ответе OK API.
    /// </summary>
    [JsonPropertyName("author_ref")]
    public string AuthorRef { get; init; }

    /// <summary>
    /// Текст комментария. Может содержать эмодзи, упоминания или другие элементы,
    /// поддерживаемые редактором Одноклассников.
    /// Соответствует полю <c>"text"</c> в JSON.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; }

    /// <summary>
    /// Время создания комментария в миллисекундах с Unix-эпохи. Соответствует полю <c>"created_ms"</c>
    /// в API OK (не <c>"date_ms"</c> — такого поля API не возвращает). Для некоторых типов обсуждений
    /// (замечено на личных <c>USER_STATUS</c>/<c>USER_PHOTO</c>) OK не отдаёт это поле вовсе (<see langword="null"/>)
    /// — тогда используется резервный <see cref="Date"/>, см. <see cref="Timestamp"/>.
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
    public long Timestamp => CreatedMs ?? OkLegacyDateParser.ParseToUnixMs(Date);

    /// <summary>
    /// Тип комментария. В большинстве случаев — «text», но может использоваться
    /// для расширения (например, системные уведомления).
    /// Соответствует полю <c>"type"</c> в JSON-ответе.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; }

    /// <summary>
    /// Идентификатор комментария, на который дан ответ (в случае вложенных комментариев).
    /// Поле может отсутствовать или быть пустым, если комментарий не является ответом.
    /// Соответствует <c>"reply_to_comment_id"</c> в API.
    /// </summary>
    [JsonPropertyName("reply_to_comment_id")]
    public string ReplyToCommentId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, которому адресован ответ (например, при ответе на чужой комментарий).
    /// Соответствует полю <c>"reply_to_id"</c> в JSON.
    /// </summary>
    [JsonPropertyName("reply_to_id")]
    public string ReplyToId { get; init; }

    /// <summary>
    /// Имя пользователя, которому адресован ответ (для отображения в интерфейсе).
    /// Соответствует полю <c>"reply_to_name"</c> в ответе API.
    /// </summary>
    [JsonPropertyName("reply_to_name")]
    public string ReplyToName { get; init; }
}

