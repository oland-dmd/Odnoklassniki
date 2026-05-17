using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;

/// <summary>
/// Информация об изображении-превью внутри вложения ссылки.
/// Вложен в <see cref="LinkAttachmentData"/>, поле <c>image</c>.
/// </summary>
public sealed record LinkImageData : BaseOkDto
{
    /// <summary>URL изображения.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>Ширина изображения в пикселях.</summary>
    [JsonPropertyName("width")]
    public short? Width { get; init; }

    /// <summary>Высота изображения в пикселях.</summary>
    [JsonPropertyName("height")]
    public short? Height { get; init; }

    /// <summary>Идентификатор изображения.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>Идентификатор фотографии OK.ru.</summary>
    [JsonPropertyName("photoId")]
    public string? PhotoId { get; init; }
}
