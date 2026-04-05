namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

/// <summary>
/// Представляет данные комментария из социальной сети «Одноклассники» (OK.ru).
/// Используется для десериализации ответов API и передачи данных между слоями приложения.
/// </summary>
public record CommentData
{
    /// <summary>
    /// Уникальный идентификатор комментария в системе Одноклассников.
    /// Соответствует полю <c>commentId</c> в ответе API.
    /// </summary>
    public string ID { get; init; }

    /// <summary>
    /// Идентификатор автора комментария — ID пользователя в Одноклассниках.
    /// Соответствует полю <c>userId</c> или <c>authorId</c> в API.
    /// </summary>
    public string AuthorId { get; init; }

    /// <summary>
    /// Полное имя автора комментария (например, "Иван Петров").
    /// Получается из профиля пользователя через API.
    /// </summary>
    public string AuthorName { get; init; }

    /// <summary>
    /// Короткая ссылка на профиль автора (например, "ok.ru/id123456789" или "ok.ru/ivan_petrov").
    /// Может использоваться для формирования гиперссылок в интерфейсе.
    /// </summary>
    public string AuthorRef { get; init; }

    /// <summary>
    /// Текст комментария. Может содержать упоминания, смайлы или форматирование,
    /// поддерживаемое Одноклассниками.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Время создания комментария в формате Unix time (в миллисекундах).
    /// Соответствует полю <c>replyTime</c> или <c>time</c> в API OK.
    /// </summary>
    public long Timestamp { get; init; }

    /// <summary>
    /// Тип комментария. В API Одноклассников может указываться неявно,
    /// но поле сохранено для совместимости с другими платформами.
    /// Примеры значений: "text", "media", "system".
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Идентификатор родительского комментария, если текущий является ответом.
    /// В API Одноклассников соответствует полю <c>replyToCommentId</c> (если поддерживается вложенность).
    /// Может быть <see langword="null"/> или пустой строкой.
    /// </summary>
    public string ReplyToCommentId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, на чей комментарий или пост был оставлен ответ.
    /// В Одноклассниках часто совпадает с <see cref="AuthorId"/> родительского комментария.
    /// </summary>
    public string ReplyToId { get; init; }

    /// <summary>
    /// Имя пользователя, которому адресован ответ (например, при упоминании или ответе).
    /// Используется для отображения в интерфейсе ("в ответ на Иван Петров").
    /// </summary>
    public string ReplyToName { get; init; }
}