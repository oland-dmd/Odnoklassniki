using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

/// <summary>
/// DTO пользователя, поставившего лайк на фотографию или альбом в OK.ru.
/// Используется в методах <c>photos.getPhotoLikes</c> и <c>photos.getAlbumLikes</c>.
/// </summary>
public sealed record UserLikeDto : BaseOkDto
{
    /// <summary>Идентификатор пользователя.</summary>
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }
}
