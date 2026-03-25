using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;
using Odnoklassniki.Rest.ApiClients.PhotosV2.Responses;

namespace Odnoklassniki.Rest.ApiClients.PhotosV2;

/// <summary>
/// Клиент для взаимодействия с API фотографий (версия 2) социальной сети Одноклассники (OK.ru).
/// </summary>
public class PhotosV2ApiClient(IOkApiClientCore okApi) : IPhotosV2ApiClient
{
    private const string OkClassName = "photosV2";
    private const string GetUploadUrlMethodName = $"{OkClassName}.getUploadUrl";
    private const string CommitMethodName = $"{OkClassName}.commit";

    // === PUBLIC API: Upload URL Methods ===

    public Task<UploadUrlData> GetUploadUrlForUserAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default)
    {
        return GetUploadUrlInternalAsync(accessToken, sessionSecretKey, albumId: null, groupId: null, cancellationToken);
    }

    public Task<UploadUrlData> GetUploadUrlForUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(albumId))
            throw new ArgumentException("Album ID cannot be empty", nameof(albumId));
        
        return GetUploadUrlInternalAsync(accessToken, sessionSecretKey, albumId: albumId, groupId: null, cancellationToken);
    }

    public Task<UploadUrlData> GetUploadUrlForGroupAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return GetUploadUrlInternalAsync(accessToken, sessionSecretKey, albumId: null, groupId: groupId, cancellationToken);
    }

    public Task<UploadUrlData> GetUploadUrlForGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        if (string.IsNullOrWhiteSpace(albumId))
            throw new ArgumentException("Album ID cannot be empty", nameof(albumId));
        
        return GetUploadUrlInternalAsync(accessToken, sessionSecretKey, albumId: albumId, groupId: groupId, cancellationToken);
    }

    // === PUBLIC API: Commit ===

    public async Task<ICollection<CommitPhotoData>> CommitAsync(
        string accessToken,
        string sessionSecretKey,
        string comment,
        string photoId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("comment", comment)
            .InsertCustomParameter("photo_id", photoId)
            .InsertCustomParameter("token", token);

        var response = await okApi.CallAsync<CommitResponse>(
            CommitMethodName,
            accessToken,
            sessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);

        return response.Photos.Select(item => new CommitPhotoData
        {
            Id = item.Id,
            Status = item.Status
        }).ToArray();
    }

    // === INTERNAL IMPLEMENTATION ===

    /// <summary>
    /// Внутренняя реализация получения URL для загрузки.
    /// Инкапсулирует логику формирования параметров и работу с пустыми значениями.
    /// </summary>
    private async Task<UploadUrlData> GetUploadUrlInternalAsync(
        string accessToken,
        string sessionSecretKey,
        string? albumId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertCount(1);

        // Добавляем параметры только если они не пустые — избегаем отправки "" в API
        if (!string.IsNullOrWhiteSpace(groupId))
        {
            parameters = parameters.InsertGroupId(groupId);
        }

        if (!string.IsNullOrWhiteSpace(albumId))
        {
            parameters = parameters.InsertAlbumId(albumId);
        }

        var response = await okApi.CallAsync<UploadPhotoResponse>(
            GetUploadUrlMethodName,
            accessToken,
            sessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);

        return new UploadUrlData
        {
            PhotoIds = response.PhotoIds,
            UploadUrl = response.UploadUrl
        };
    }
}