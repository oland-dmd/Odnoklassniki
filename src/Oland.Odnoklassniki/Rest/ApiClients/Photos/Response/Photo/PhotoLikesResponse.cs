using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на запросы <c>photos.getPhotoLikes</c> и <c>photos.getAlbumLikes</c>.
/// </summary>
internal sealed record PhotoLikesResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>Курсор для получения следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Флаг наличия дополнительных страниц.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество пользователей, поставивших лайк.</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; init; }

    /// <summary>Список пользователей, поставивших лайк.</summary>
    [JsonPropertyName("users")]
    public ICollection<TDto>? Users { get; init; }
}
