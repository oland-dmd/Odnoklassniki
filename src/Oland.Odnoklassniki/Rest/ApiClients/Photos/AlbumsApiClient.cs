using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Album;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos;

/// <summary>
/// Клиент для управления альбомами фотографий в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class AlbumsApiClient(IOkApiClientCore okApi) : IAlbumsApiClient
{
    private const string OkClassName = "photos";

    private static readonly ICollection<string> DefaultFields = 
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

    private const string SetAlbumMainPhotoMethodName = $"{OkClassName}.setAlbumMainPhoto";

    /// <inheritdoc />
    public async Task SetAlbumMainPhotoAsync(string albumId,
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertPhotoId(photoId);

        switch (context)
        {
            case GroupRequestContext or
                MainAccountRequestContext or
                ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
        }

        context.Deconstruct(out var accessToken, out var sessionSecretKey);
        await okApi.CallAsync(
            SetAlbumMainPhotoMethodName,
            accessToken,
            sessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);
    }

    private const string CreateAlbumMethodName = $"{OkClassName}.createAlbum";

    /// <inheritdoc />
    public async Task<string> CreateAlbumAsync(
        string title,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertTitle(title)
            .InsertCustomParameter("type", "PUBLIC");

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<string>(CreateAlbumMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);
        if (response is null)
        {
            throw new OkApiException("Failed to create album", 500);
        }
        
        return response.Trim('"');
    }

    private const string DeleteAlbumMethodName = $"{OkClassName}.deleteAlbum";


    /// <inheritdoc />
    public async Task DeleteAlbumAsync(string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        await okApi.CallAsync<string>(DeleteAlbumMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    #region Internal Helpers

    private const string GetAlbumsMethodName = $"{OkClassName}.getAlbums";

    private async Task<AnchorResponse<AlbumData>> GetAlbumsInternalAsync(
        IRequestContext context,
        AnchorConfiguration configuration,
        string[] fields,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCount(configuration.Count)
            .InsertPagingAnchor(configuration.Anchor)
            .InsertPagingDirection(configuration.Direction)
            .InsertFields(fields?.Length > 0 ? fields : DefaultFields);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(FriendRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<AlbumsResponse>(GetAlbumsMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);

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

    #endregion


    private const string EditAlbumMethodName = $"{OkClassName}.editAlbum";

    /// <inheritdoc />
    public async Task EditAlbumAsync(string albumId,
        string title,
        string description,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertTitle(title)
            .InsertDescription(description);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        await okApi.CallAsync(EditAlbumMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public AnchorNavigator<AlbumData> GetAlbumsNavigatorAsync(
        IRequestContext context,
        AnchorConfiguration configuration,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        return new AnchorNavigator<AlbumData>(
            nextAnchor => GetAlbumsInternalAsync(
                context, nextAnchor, fields, cancellationToken)
                , configuration);
    }

    private const string GetAlbumInfoMethodName = $"{OkClassName}.getAlbumInfo";

    /// <inheritdoc />
    public async Task<AlbumData?> GetAlbumInfoAsync(
        string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default,
        params string[] fields)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertFields(fields?.Length > 0 ? fields : DefaultFields);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(FriendRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<AlbumResponse>(GetAlbumInfoMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);

        if (response?.Album == null) return null;

        return new AlbumData
        {
            Id = response.Album.Id,
            Title = response.Album.Title,
            UserId = response.Album.UserId
        };
    }
}