using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

/// <summary>
/// DTO оценки, выставленной текущим пользователем чужой фотографии.
/// Используется в методе <c>photos.getPhotoMarks</c> (устаревшем).
/// Поля соответствуют структуре элемента массива <c>photomark</c> в ответе API.
/// </summary>
public sealed record PhotoMarkDto : BaseOkDto
{
    /// <summary>Числовая оценка фотографии (обычно от 1 до 5).</summary>
    [JsonPropertyName("mark")]
    public int Mark { get; init; }

    /// <summary>Идентификатор пользователя, выставившего оценку.</summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>Время выставления оценки (Unix-миллисекунды).</summary>
    [JsonPropertyName("date_ms")]
    public long? DateMs { get; init; }
}
