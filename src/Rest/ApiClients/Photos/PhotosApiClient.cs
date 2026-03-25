using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;

namespace Odnoklassniki.Rest.ApiClients.Photos;

/// <summary>
/// Клиент для работы с фотографиями в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class PhotosApiClient(IOkApiClientCore okApi) : IPhotosApiClient
{
    private const string OkClassName = "photos";

    #region Get Photos List

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetUserPhotosAsync(
        string albumId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithDefaultCredentialsAsync(albumId, friendId: null, groupId: null, anchor, count, cancellationToken));
    }

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetFriendPhotosAsync(
        string albumId,
        string friendId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(friendId))
            throw new ArgumentException("Friend ID cannot be empty", nameof(friendId));
        
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithDefaultCredentialsAsync(albumId, friendId, groupId: null, anchor, count, cancellationToken));
    }

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetGroupPhotosAsync(
        string albumId,
        string groupId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithDefaultCredentialsAsync(albumId, friendId: null, groupId, anchor, count, cancellationToken));
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetUserPhotosAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithExplicitCredentialsAsync(accessToken, sessionSecretKey, albumId, friendId: null, groupId: null, anchor, count, cancellationToken));
    }

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetFriendPhotosAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string friendId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(friendId))
            throw new ArgumentException("Friend ID cannot be empty", nameof(friendId));
        
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithExplicitCredentialsAsync(accessToken, sessionSecretKey, albumId, friendId, groupId: null, anchor, count, cancellationToken));
    }

    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetGroupPhotosAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string groupId,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return new AnchorNavigator<PhotoData>(anchor => GetPhotosWithExplicitCredentialsAsync(accessToken, sessionSecretKey, albumId, friendId: null, groupId, anchor, count, cancellationToken));
    }

    #endregion

    #region Get Photo Info

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task<PhotoData> GetUserPhotoInfoAsync(
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return GetPhotoInfoWithDefaultCredentialsAsync(photoId, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PhotoData> GetGroupPhotoInfoAsync(
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return GetPhotoInfoWithDefaultCredentialsAsync(photoId, groupId, cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task<PhotoData> GetUserPhotoInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return GetPhotoInfoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PhotoData> GetGroupPhotoInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return GetPhotoInfoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, groupId, cancellationToken);
    }

    #endregion

    #region Edit Photo

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task<bool> EditUserPhotoAsync(
        string photoId,
        string description,
        CancellationToken cancellationToken = default)
    {
        return EditPhotoWithDefaultCredentialsAsync(photoId, description, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> EditGroupPhotoAsync(
        string photoId,
        string description,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return EditPhotoWithDefaultCredentialsAsync(photoId, description, groupId, cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task<bool> EditUserPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        CancellationToken cancellationToken = default)
    {
        return EditPhotoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, description, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> EditGroupPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return EditPhotoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, description, groupId, cancellationToken);
    }

    #endregion

    #region Delete Photo

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task DeleteUserPhotoAsync(
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return DeletePhotoWithDefaultCredentialsAsync(photoId, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteGroupPhotoAsync(
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return DeletePhotoWithDefaultCredentialsAsync(photoId, groupId, cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task DeleteUserPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return DeletePhotoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, groupId: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteGroupPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return DeletePhotoWithExplicitCredentialsAsync(accessToken, sessionSecretKey, photoId, groupId, cancellationToken);
    }

    #endregion

    #region Internal Helpers: Default Credentials (Main Account)

    private const string GetPhotosMethodName = $"{OkClassName}.getPhotos";

    private async Task<AnchorResponse<PhotoData>> GetPhotosWithDefaultCredentialsAsync(
        string albumId,
        string? friendId,
        string? groupId,
        string anchor,
        int count,
        CancellationToken cancellationToken)
    {
        // Пустые строки передаются ТОЛЬКО здесь — инкапсулировано от внешнего кода
        return await GetPhotosCoreAsync("", "", albumId, friendId, groupId, anchor, count, cancellationToken);
    }

    private async Task<PhotoData> GetPhotoInfoWithDefaultCredentialsAsync(
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        return await GetPhotoInfoCoreAsync("", "", photoId, groupId, cancellationToken);
    }

    private async Task<bool> EditPhotoWithDefaultCredentialsAsync(
        string photoId,
        string description,
        string? groupId,
        CancellationToken cancellationToken)
    {
        return await EditPhotoCoreAsync("", "", photoId, description, groupId, cancellationToken);
    }

    private async Task DeletePhotoWithDefaultCredentialsAsync(
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        await DeletePhotoCoreAsync("", "", photoId, groupId, cancellationToken);
    }

    #endregion

    #region Internal Helpers: Explicit Credentials

    private async Task<AnchorResponse<PhotoData>> GetPhotosWithExplicitCredentialsAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string? friendId,
        string? groupId,
        string anchor,
        int count,
        CancellationToken cancellationToken)
    {
        return await GetPhotosCoreAsync(accessToken, sessionSecretKey, albumId, friendId, groupId, anchor, count, cancellationToken);
    }

    private async Task<PhotoData> GetPhotoInfoWithExplicitCredentialsAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        return await GetPhotoInfoCoreAsync(accessToken, sessionSecretKey, photoId, groupId, cancellationToken);
    }

    private async Task<bool> EditPhotoWithExplicitCredentialsAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        string? groupId,
        CancellationToken cancellationToken)
    {
        return await EditPhotoCoreAsync(accessToken, sessionSecretKey, photoId, description, groupId, cancellationToken);
    }

    private async Task DeletePhotoWithExplicitCredentialsAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        await DeletePhotoCoreAsync(accessToken, sessionSecretKey, photoId, groupId, cancellationToken);
    }

    #endregion

    #region Core Implementation (Single Source of Truth)

    private async Task<AnchorResponse<PhotoData>> GetPhotosCoreAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string? friendId,
        string? groupId,
        string anchor,
        int count,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "photo.pic_max", "photo.id", "photo.text", "photo.user_Id",
                "group_photo.pic_max", "group_photo.id", "group_photo.text", "group_photo.user_Id")
            .InsertCount(count)
            .InsertAnchor(anchor);
        
        if (!string.IsNullOrWhiteSpace(friendId))
            parameters = parameters.InsertFriendId(friendId);
        
        if (!string.IsNullOrWhiteSpace(albumId))
            parameters = parameters.InsertAlbumId(albumId);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        var response = await okApi.CallAsync<PhotosResponse>(
            GetPhotosMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<PhotoData>()
        {
            Anchor = response.Anchor,
            HasMore = response.HasMore,
            TotalCount = response.TotalCount,
            Results = response.Photos.Select(item => new PhotoData
            {
                Id = item.Id,
                PicMax = item.PicMax,
                Text = item.Text,
                UserId = item.UserId,
            }).ToArray()
        };
    }

    private const string GetPhotoInfoMethodName = $"{OkClassName}.getPhotoInfo";
    private async Task<PhotoData> GetPhotoInfoCoreAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertFields(
                "photo.pic_max", "photo.id", "photo.text", "photo.user_Id",
                "group_photo.pic_max", "group_photo.id", "group_photo.text", "group_photo.user_Id");
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        var response = await okApi.CallAsync<PhotoResponse>(
            GetPhotoInfoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return new PhotoData
        {
            Id = response.Photo.Id,
            PicMax = response.Photo.PicMax,
            Text = response.Photo.Text,
            UserId = response.Photo.UserId
        };
    }

    private const string EditPhotosMethodName = $"{OkClassName}.editPhoto";
    private async Task<bool> EditPhotoCoreAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        string? groupId,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertDescription(description);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        return await okApi.CallAsync<bool>(
            EditPhotosMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    private const string DeletePhotoMethodName = $"{OkClassName}.deletePhoto";
    private async Task DeletePhotoCoreAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string? groupId,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        await okApi.CallAsync(
            DeletePhotoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    #endregion
}