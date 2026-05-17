using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

/// <summary>
/// Ответ API Одноклассников на пакетный запрос <c>photos.getInfo</c> с параметром <c>photo_ids</c>.
/// </summary>
internal sealed record PhotosBatchInfoResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>Коллекция фотографий с запрошенными полями.</summary>
    [JsonPropertyName("photos")]
    public ICollection<TDto>? Photos { get; init; }
}
