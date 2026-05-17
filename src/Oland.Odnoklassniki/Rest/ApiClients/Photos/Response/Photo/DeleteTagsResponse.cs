using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на запрос <c>photos.deleteTags</c>.
/// </summary>
internal sealed record DeleteTagsResponse
{
    /// <summary>Количество успешно удалённых тегов.</summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
