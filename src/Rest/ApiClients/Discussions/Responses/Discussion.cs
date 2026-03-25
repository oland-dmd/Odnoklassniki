using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Представляет обсуждение (дискуссию) из API «Одноклассников», полученное, например,
/// при вызове метода <c>discussions.get</c> или аналогичного.
/// Обсуждение привязано к определённому объекту — альбому, посту, фото и т.д.
/// </summary>
internal record Discussion
{
    /// <summary>
    /// Уникальный идентификатор объекта, к которому привязано обсуждение.
    /// В Одноклассниках обсуждения не имеют собственного ID, а используют ID связанного объекта
    /// (например, альбома или медиатопика).
    /// Соответствует полю <c>"object_id"</c> в JSON-ответе API.
    /// </summary>
    [JsonPropertyName("object_id")]
    public string ID { get; init; }

    /// <summary>
    /// Заголовок обсуждения. Обычно совпадает с названием связанного объекта
    /// (например, названием альбома или текстом поста).
    /// Отображается в интерфейсе приложения.
    /// Соответствует полю <c>"title"</c> в ответе OK API.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; }

    /// <summary>
    /// Количество новых (непрочитанных) комментариев в этом обсуждении для текущего пользователя.
    /// Используется для отображения индикаторов активности (например, «3 новых комментария»).
    /// Соответствует полю <c>"new_comments_count"</c> в JSON.
    /// </summary>
    [JsonPropertyName("new_comments_count")]
    public int NewCommentCount { get; init; }

    /// <summary>
    /// Тип объекта, к которому привязано обсуждение.
    /// Возможные значения: <c>"ALBUM"</c>, <c>"GROUP"</c>, <c>"PHOTO"</c>, <c>"VIDEO"</c>, <c>"USER_POST"</c> и др.
    /// Определяет контекст отображения и логику обработки.
    /// Соответствует полю <c>"object_type"</c> в API OK.
    /// </summary>
    [JsonPropertyName("object_type")]
    public string Type { get; init; }

    /// <summary>
    /// Массив ссылочных объектов, связанных с обсуждением.
    /// В Одноклассниках это может включать, например, информацию о группе (если обсуждение в сообществе)
    /// или владельце альбома. Используется для получения дополнительных метаданных без отдельных запросов.
    /// Соответствует полю <c>"ref_objects"</c> в JSON-ответе.
    /// </summary>
    [JsonPropertyName("ref_objects")]
    public RefObject[] RefObjects { get; init; }
}