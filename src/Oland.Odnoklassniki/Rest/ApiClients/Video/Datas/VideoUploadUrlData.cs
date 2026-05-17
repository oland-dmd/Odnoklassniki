using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Video.Datas;

/// <summary>
/// DTO с данными для загрузки видеоролика в Одноклассники.
/// Возвращается методом <c>video.getUploadUrl</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record VideoUploadUrlData : BaseOkDto
{
    /// <summary>URL-адрес для загрузки видеофайла (multipart/form-data POST).</summary>
    [JsonPropertyName("upload_url")]
    public string? UploadUrl { get; init; }

    /// <summary>Идентификатор созданного видеоролика.</summary>
    [JsonPropertyName("video_id")]
    public long? VideoId { get; init; }
}
