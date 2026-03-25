using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Представляет пагинированный ответ API «Одноклассников», содержащий список обсуждений.
/// Используется при вызове методов вроде <c>discussions.get</c>, которые возвращают
/// обсуждения с поддержкой постраничной загрузки через якорь (<c>anchor</c>).
/// </summary>
internal record DiscussionResponse
{
    /// <summary>
    /// Якорь (маркер позиции), используемый для загрузки следующей порции обсуждений.
    /// При последующем запросе это значение передаётся в параметре <c>anchor</c>,
    /// чтобы продолжить чтение списка с текущего места.
    /// Если дальнейшие данные отсутствуют, поле может быть <see langword="null"/> или пустой строкой.
    /// Соответствует полю <c>"anchor"</c> в JSON-ответе OK API.
    /// </summary>
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }

    /// <summary>
    /// Массив обсуждений, полученных в текущей порции.
    /// Каждый элемент — объект <see cref="Discussion"/>, десериализованный из JSON.
    /// Обсуждения могут быть привязаны к альбомам, группам, постам и другим типам контента.
    /// Порядок обычно определяется временем последней активности (новые комментарии — выше).
    /// </summary>
    [JsonPropertyName("discussions")]
    public Discussion[] Discussions { get; init; }
}