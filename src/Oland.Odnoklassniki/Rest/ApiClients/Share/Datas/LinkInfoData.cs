using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;

/// <summary>
/// Ответ метода <c>share.fetchLinkV2</c>.
/// Подпись для <c>mediatopic.post</c> приходит одним из двух способов: либо в массиве
/// <c>attachment_media</c> (карточки с несколькими медиа-вариантами), либо в объекте
/// <c>link</c> (одиночное превью страницы, например, товара маркета) — на практике для
/// ссылок на страницы ok.ru пришёл именно <c>link</c>, а <c>attachment_media</c> был <c>null</c>.
/// Нераспознанные поля (например, <c>entities</c>) доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record LinkInfoData : BaseOkDto
{
    /// <summary>
    /// Список вариантов вложения ссылки. Потребитель выбирает нужный элемент и передаёт
    /// его <c>signature</c> и <c>mediaIdx</c> при вызове <c>mediatopic.post</c>.
    /// </summary>
    [JsonPropertyName("attachment_media")]
    public ICollection<LinkAttachmentData>? AttachmentMedia { get; init; }

    /// <summary>Превью ссылки с подписью — заполняется, когда <see cref="AttachmentMedia"/> пуст.</summary>
    [JsonPropertyName("link")]
    public LinkPreviewData? Link { get; init; }
}
