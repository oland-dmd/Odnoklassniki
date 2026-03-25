using Odnoklassniki.Enums;
using Odnoklassniki.Exceptions;
using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Odnoklassniki.Rest.ApiClients.Photos.Response.Album;

namespace Odnoklassniki.Rest.ApiClients.Photos;

/// <summary>
/// Клиент для управления альбомами фотографий в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class AlbumsApiClient(IOkApiClientCore okApi) : IAlbumsApiClient
{
    private const string OkClassName = "photos";

    private static ICollection<string> DefaultFields = 
        [
            "album.title",
            "album.aid",
            "album.user_id",
            "album.ADD_PHOTO_ALLOWED",
            "group_album.title",
            "group_album.aid",
            "group_album.user_id",
            "group_album.ADD_PHOTO_ALLOWED"
        ]; 

    #region Set Main Photo

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task SetUserAlbumMainPhotoAsync(
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return SetAlbumMainPhotoInternalAsync(albumId, photoId, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task SetGroupAlbumMainPhotoAsync(
        string groupId,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return SetAlbumMainPhotoInternalAsync(albumId, photoId, groupId, cancellationToken: cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task SetUserAlbumMainPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        return SetAlbumMainPhotoInternalAsync(albumId, photoId, groupId: null, accessToken: accessToken, sessionSecretKey: sessionSecretKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task SetGroupAlbumMainPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return SetAlbumMainPhotoInternalAsync(albumId, photoId, groupId, accessToken, sessionSecretKey, cancellationToken);
    }

    #endregion

    #region Create Album

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task<string> CreateUserAlbumAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        return CreateAlbumInternalAsync(title, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> CreateGroupAlbumAsync(
        string groupId,
        string title,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return CreateAlbumInternalAsync(title, groupId, cancellationToken: cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task<string> CreateUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string title,
        CancellationToken cancellationToken = default)
    {
        return CreateAlbumInternalAsync(title, accessToken: accessToken, sessionSecretKey: sessionSecretKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> CreateGroupAlbumAsync(string accessToken,
        string sessionSecretKey,
        string groupId,
        string title,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return CreateAlbumInternalAsync(title, groupId, accessToken, sessionSecretKey, cancellationToken);
    }

    #endregion

    #region Delete Album

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task DeleteUserAlbumAsync(
        string albumId,
        CancellationToken cancellationToken = default)
    {
        return DeleteAlbumInternalAsync(albumId, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteGroupAlbumAsync(
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return DeleteAlbumInternalAsync(albumId, groupId, cancellationToken: cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task DeleteUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default)
    {
        return DeleteAlbumInternalAsync(albumId, accessToken: accessToken, sessionSecretKey: sessionSecretKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return DeleteAlbumInternalAsync(albumId, groupId, accessToken, sessionSecretKey, cancellationToken);
    }

    #endregion

    #region Edit Album

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task EditUserAlbumAsync(
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        return EditAlbumInternalAsync(albumId, title, description, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task EditGroupAlbumAsync(
        string groupId,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(groupId) 
            ? throw new ArgumentException("Group ID cannot be empty", nameof(groupId))
            : EditAlbumInternalAsync(albumId, title, groupId: groupId, description: description, cancellationToken: cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task EditUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        return EditAlbumInternalAsync(albumId, title, description, sessionSecretKey: sessionSecretKey, accessToken: accessToken, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task EditGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return EditAlbumInternalAsync(albumId, title, description, groupId, accessToken, sessionSecretKey, cancellationToken);
    }

    #endregion

    #region Get Albums List

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetUserAlbumsAsync(
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        return new AnchorNavigator<AlbumData>(
            passedAnchor => GetAlbumsInternalAsync(count: count,
                direction: direction,
                cancellationToken: cancellationToken,
                anchor: passedAnchor,
                fields: fields), anchor);
    }

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetGroupAlbumsNavigator(
        string groupId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return new AnchorNavigator<AlbumData>(passedAnchor => GetAlbumsInternalAsync(
            groupId: groupId, count: count, direction: direction, cancellationToken: cancellationToken, anchor: passedAnchor, fields: fields), anchor);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetUserAlbumsAsync(
        string accessToken,
        string sessionSecretKey,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        return new AnchorNavigator<AlbumData>(passedAnchor => GetAlbumsInternalAsync(
            accessToken: accessToken, sessionSecretKey: sessionSecretKey, 
            count: count, direction: direction, cancellationToken: cancellationToken, anchor: passedAnchor, fields: fields), anchor);
    }

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetFriendAlbumsAsync(
        string accessToken,
        string sessionSecretKey,
        string friendId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(friendId))
            throw new ArgumentException("Friend ID cannot be empty", nameof(friendId));
        
        return new AnchorNavigator<AlbumData>(passedAnchor => GetAlbumsInternalAsync(
            accessToken: accessToken, sessionSecretKey: sessionSecretKey, 
            friendId:friendId, count: count, direction: direction, cancellationToken: cancellationToken, anchor: passedAnchor, fields: fields), anchor);
    }

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetGroupAlbumsNavigator(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return new AnchorNavigator<AlbumData>(passedAnchor => GetAlbumsInternalAsync(
            anchor: passedAnchor,
            count: count,
            direction: direction,
            fields: fields,
            accessToken: accessToken,
            sessionSecretKey: sessionSecretKey,
            groupId: groupId,
            cancellationToken: cancellationToken), anchor);
    }

    #endregion

    #region Get Album Info

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task<AlbumData?> GetUserAlbumInfoAsync(
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        return GetAlbumInfoInternalAsync(
            albumId,
            fields,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task<AlbumData?> GetGroupAlbumInfoAsync(
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return GetAlbumInfoInternalAsync(
            albumId,
            fields,
            groupId: groupId,
            cancellationToken: cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task<AlbumData?> GetUserAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        return GetAlbumInfoInternalAsync(
            albumId,
            fields,
            accessToken: accessToken,
            sessionSecretKey: sessionSecretKey,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task<AlbumData?> GetFriendAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string friendId,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(friendId))
            throw new ArgumentException("Friend ID cannot be empty", nameof(friendId));
        
        return GetAlbumInfoInternalAsync(
            albumId,
            fields,
            friendId,
            accessToken: accessToken,
            sessionSecretKey: sessionSecretKey,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task<AlbumData?> GetGroupAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        
        return GetAlbumInfoInternalAsync(
            albumId,
            fields,
            accessToken: accessToken,
            groupId: groupId,
            sessionSecretKey: sessionSecretKey,
            cancellationToken: cancellationToken);
    }

    #endregion

    #region Internal Helpers

    private const string SetAlbumMainPhotoMethodName = $"{OkClassName}.setAlbumMainPhoto";

    private async Task SetAlbumMainPhotoInternalAsync(
        string albumId,
        string photoId,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertPhotoId(photoId);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        await okApi.CallAsync(SetAlbumMainPhotoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    private const string CreateAlbumMethodName = $"{OkClassName}.createAlbum";

    private async Task<string> CreateAlbumInternalAsync(
        string title,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertTitle(title)
            .InsertCustomParameter("type", "PUBLIC");
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        var response = await okApi.CallAsync<string>(CreateAlbumMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
        if (response is null)
        {
            throw new OkApiException("Failed to create album", 500);
        }
        
        return response.Trim('"');
    }

    private const string DeleteAlbumMethodName = $"{OkClassName}.deleteAlbum";

    private async Task DeleteAlbumInternalAsync(
        string albumId,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        await okApi.CallAsync<string>(DeleteAlbumMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    private const string EditAlbumMethodName = $"{OkClassName}.editAlbum";

    private async Task EditAlbumInternalAsync(
        string albumId,
        string title,
        string description,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertTitle(title)
            .InsertDescription(description);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);

        await okApi.CallAsync(EditAlbumMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    private const string GetAlbumsMethodName = $"{OkClassName}.getAlbums";

    private async Task<AnchorResponse<AlbumData>> GetAlbumsInternalAsync(
        string anchor,
        int count,
        PagingDirection direction,
        string[] fields,
        string? friendId = null,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCount(count)
            .InsertPagingAnchor(anchor)
            .InsertPagingDirection(direction);
        
        parameters = parameters.InsertFields(fields?.Length > 0 ? fields : DefaultFields);
        
        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);
        
        if (!string.IsNullOrWhiteSpace(friendId))
            parameters = parameters.InsertFriendId(friendId);

        var response = await okApi.CallAsync<AlbumsResponse>(GetAlbumsMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        if (response == null) return null;

        return new AnchorResponse<AlbumData>()
        {
            Anchor = response.Anchor,
            Results = response.Albums.Select(item => new AlbumData
            {
                Id = item.Id,
                Title = item.Title,
                UserId = item.UserId,
                IsAddPhotoAllowed = item.Attributes?.Flags == "ap"
            }).ToArray(),
            HasMore = response.HasMore,
            TotalCount = response.TotalCount
        };
    }

    private const string GetAlbumInfoMethodName = $"{OkClassName}.getAlbumInfo";

    private async Task<AlbumData?> GetAlbumInfoInternalAsync(
        string albumId,
        string[] fields,
        string? friendId = null,
        string? groupId = null,
        string accessToken = "",
        string sessionSecretKey = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId);

        parameters = parameters.InsertFields(fields?.Length > 0 ? fields : DefaultFields);

        if (!string.IsNullOrWhiteSpace(groupId))
            parameters = parameters.InsertGroupId(groupId);
        
        if (!string.IsNullOrWhiteSpace(friendId))
            parameters = parameters.InsertFriendId(friendId);

        var response = await okApi.CallAsync<AlbumResponse>(GetAlbumInfoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        if (response?.Album == null) return null;

        return new AlbumData
        {
            Id = response.Album.Id,
            Title = response.Album.Title,
            UserId = response.Album.UserId
        };
    }

    #endregion
}