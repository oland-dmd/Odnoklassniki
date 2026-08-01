using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Link;

public class LinkMedia : MediaItem
{
    [JsonPropertyName("url")] public required string Url { get; set; }

    /// <summary>
    /// Подпись вложения из <c>share.fetchLinkV2</c>. Обязательна для ссылок на внутренние
    /// страницы ok.ru (например, товары маркета) — без неё <c>mediatopic.post</c> отклоняет
    /// их с ошибкой <c>errors.web-grabber.internal-link-failure</c>.
    /// </summary>
    [JsonPropertyName("signature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Signature { get; set; }

    [JsonPropertyName("mediaIdx")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MediaIdx { get; set; }
}