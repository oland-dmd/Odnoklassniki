using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;

/// <summary>
/// Элемент массива <c>attachment_media</c> в ответе <c>share.fetchLinkV2</c>.
/// Содержит <c>signature</c> и <c>mediaIdx</c>, необходимые для публикации ссылки
/// через <c>mediatopic.post</c>.
/// </summary>
public sealed record LinkAttachmentData : BaseOkDto
{
    /// <summary>Тип вложения (например, <c>link</c>, <c>adlink</c>).</summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Подпись вложения. Обязательно передавать в <c>mediatopic.post</c> при публикации ссылки.
    /// </summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; init; }

    /// <summary>Индекс медиа для выбора изображения при публикации.</summary>
    [JsonPropertyName("mediaIdx")]
    public int? MediaIdx { get; init; }

    /// <summary>URL страницы.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>Заголовок страницы.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>Описание страницы.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>URL превью-изображения.</summary>
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    /// <summary>Объект превью-изображения с размерами и идентификаторами.</summary>
    [JsonPropertyName("image")]
    public LinkImageData? Image { get; init; }

    /// <summary>Текст кнопки действия (если есть).</summary>
    [JsonPropertyName("buttonTitle")]
    public string? ButtonTitle { get; init; }

    /// <summary>Ключ кнопки действия.</summary>
    [JsonPropertyName("buttonKey")]
    public string? ButtonKey { get; init; }

    /// <summary>Тип кнопки действия.</summary>
    [JsonPropertyName("buttonType")]
    public string? ButtonType { get; init; }
}
