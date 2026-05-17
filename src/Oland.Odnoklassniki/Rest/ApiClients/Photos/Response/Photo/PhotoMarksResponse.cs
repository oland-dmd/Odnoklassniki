using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на устаревший запрос <c>photos.getPhotoMarks</c>.
/// Возвращает оценки, выставленные текущим пользователем чужим фотографиям.
/// Поле ответа — <c>photomark</c> (не <c>marks</c>).
/// </summary>
internal sealed record PhotoMarksResponse
{
    /// <summary>Список оценок текущего пользователя.</summary>
    [JsonPropertyName("photomark")]
    public ICollection<PhotoMarkDto>? Marks { get; init; }

    /// <summary>Курсор для следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Флаг наличия следующей страницы.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество оценок.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}
