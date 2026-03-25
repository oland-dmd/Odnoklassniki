using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Представляет ответ API Одноклассников, содержащий пакет комментариев к обсуждению.
/// Используется при вызове методов вроде <c>mediatopic.getComments</c> или <c>discussions.getComments</c>.
/// Поддерживает постраничную загрузку комментариев.
/// </summary>
internal record CommentResponse
{
    /// <summary>
    /// Указывает, есть ли дополнительные комментарии на следующей странице.
    /// Если <see langword="true"/>, клиент может запросить следующую порцию,
    /// передав текущее значение <see cref="Anchor"/> как параметр пагинации.
    /// Соответствует полю <c>"has_more"</c> в JSON-ответе OK API.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// Уникальный идентификатор обсуждения, к которому относятся комментарии.
    /// Соответствует полю <c>"discussionId"</c> в ответе API.
    /// Может использоваться для сопоставления с локальной моделью обсуждения.
    /// </summary>
    [JsonPropertyName("discussionId")]
    public string DiscussionId { get; init; }

    /// <summary>
    /// Тип обсуждения (например, "ALBUM", "GROUP_POST", "USER_PHOTO" и т.д.).
    /// Определяет контекст, в котором было создано обсуждение.
    /// Соответствует полю <c>"discussionType"</c> в JSON-ответе.
    /// </summary>
    [JsonPropertyName("discussionType")]
    public string DiscussionType { get; init; }

    /// <summary>
    /// Якорь (маркер позиции), используемый для загрузки следующей страницы комментариев.
    /// При последующем запросе этот параметр передаётся в API как <c>anchor</c>,
    /// чтобы продолжить чтение с текущего места.
    /// Соответствует полю <c>"anchor"</c> в ответе OK.ru.
    /// </summary>
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }

    /// <summary>
    /// Массив комментариев, полученных в текущей порции.
    /// Каждый элемент — объект <see cref="Comment"/>, десериализованный из JSON.
    /// Порядок обычно соответствует хронологическому (от новых к старым или наоборот — зависит от параметров запроса).
    /// </summary>
    [JsonPropertyName("comments")]
    public Comment[] Comments { get; init; }
}