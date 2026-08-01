using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;

/// <summary>
/// Превью ссылки — поле <c>link</c> в ответе <c>share.fetchLinkV2</c>.
/// Заполняется для одиночных страниц (например, товаров маркета), когда
/// <see cref="LinkInfoData.AttachmentMedia"/> пуст.
/// </summary>
public sealed record LinkPreviewData : BaseOkDto
{
    /// <summary>Заголовок страницы.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>Описание страницы.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>URL страницы.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>Альтернативный URL (например, страница группы без карточки товара).</summary>
    [JsonPropertyName("alternative_url")]
    public string? AlternativeUrl { get; init; }

    /// <summary>Домен страницы.</summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    /// <summary>
    /// Подпись превью. Обязательно передавать в <c>mediatopic.post</c> при публикации ссылки.
    /// </summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; init; }
}
