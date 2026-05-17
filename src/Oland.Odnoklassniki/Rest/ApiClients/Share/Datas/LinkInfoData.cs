using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;

/// <summary>
/// Ответ метода <c>share.fetchLinkV2</c>.
/// Содержит массив вложений <c>attachment_media</c>, каждое из которых включает
/// <c>signature</c> и <c>mediaIdx</c>, необходимые для публикации в <c>mediatopic.post</c>.
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
}
