using System.Text.Json.Serialization;

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
    /// Время создания комментария в миллисекундах с Unix-эпохи.
    /// Соответствует полю <c>"date_ms"</c> в API OK.
    /// </summary>
    [JsonPropertyName("date_ms")]
    public long Timestamp { get; init; }

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

