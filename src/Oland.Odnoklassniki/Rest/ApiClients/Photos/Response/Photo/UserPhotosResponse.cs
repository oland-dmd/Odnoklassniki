using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на устаревшие запросы <c>photos.getUserPhotos</c>
/// и <c>photos.getUserAlbumPhotos</c>, использующие cursor-пагинацию через <c>pagingAnchor</c>.
/// </summary>
internal sealed record UserPhotosResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>Коллекция фотографий текущей страницы.</summary>
    [JsonPropertyName("photos")]
    public ICollection<TDto>? Photos { get; init; }

    /// <summary>Курсор пагинации для следующего запроса.</summary>
    [JsonPropertyName("pagingAnchor")]
    public string? PagingAnchor { get; init; }

    /// <summary>Флаг наличия дополнительных страниц.</summary>
    [JsonPropertyName("hasMore")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество фотографий.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}
