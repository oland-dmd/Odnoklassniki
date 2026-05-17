using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на запрос <c>photos.getTags</c>.
/// Теги фотографии возвращаются в поле <c>entities.tags</c>.
/// </summary>
internal sealed record PhotoTagsResponse
{
    /// <summary>Контейнер сущностей ответа.</summary>
    [JsonPropertyName("entities")]
    public PhotoTagsEntities? Entities { get; init; }
}

/// <summary>
/// Контейнер сущностей ответа <c>photos.getTags</c>.
/// </summary>
internal sealed record PhotoTagsEntities
{
    /// <summary>Список тегов фотографии.</summary>
    [JsonPropertyName("tags")]
    public ICollection<PhotoTagDto>? Tags { get; init; }
}
