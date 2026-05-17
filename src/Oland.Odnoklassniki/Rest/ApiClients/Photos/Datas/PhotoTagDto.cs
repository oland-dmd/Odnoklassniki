using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

/// <summary>
/// DTO для представления тега на фотографии в OK.ru.
/// Используется в методе <c>photos.getTags</c>.
/// </summary>
public sealed record PhotoTagDto : BaseOkDto
{
    /// <summary>Идентификатор тега.</summary>
    [JsonPropertyName("tag_id")]
    public string? TagId { get; init; }

    /// <summary>Идентификатор пользователя, к которому относится тег.</summary>
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    /// <summary>Текст тега (обычно имя пользователя).</summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>Левая координата X прямоугольника тега (в процентах, 0–100).</summary>
    [JsonPropertyName("x")]
    public float? X { get; init; }

    /// <summary>Верхняя координата Y прямоугольника тега (в процентах, 0–100).</summary>
    [JsonPropertyName("y")]
    public float? Y { get; init; }

    /// <summary>Правая координата X2 прямоугольника тега (в процентах, 0–100).</summary>
    [JsonPropertyName("x2")]
    public float? X2 { get; init; }

    /// <summary>Нижняя координата Y2 прямоугольника тега (в процентах, 0–100).</summary>
    [JsonPropertyName("y2")]
    public float? Y2 { get; init; }
}
