using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Album;
using Oland.Odnoklassniki.Rest.RequestContexts;
using System.Text.Json;
using Oland.Odnoklassniki.Enums;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos;

/// <summary>
/// Клиент для управления альбомами фотографий в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class AlbumsApiClient(IOkApiClientCore okApi) : IAlbumsApiClient
{
    /// <inheritdoc />
    public async Task<GroupAlbumsListing> GetCompleteGroupAlbumsAsync(IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not GroupRequestContext)
            throw new UnexpectedRequestContext(context, nameof(GroupRequestContext));

        var albums = new Dictionary<string, AlbumData>(StringComparer.Ordinal);
        var anchors = new HashSet<string>(StringComparer.Ordinal) { string.Empty };
        var anchor = string.Empty;
        // A finite read budget is distinct from evidence that the final page was reached.
        for (var pageIndex = 0; pageIndex < 1000; pageIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GroupAlbumsPageResponse? response;
            try
            {
                var parameters = context.Apply(new RestParameters()
                    .InsertCount(100)
                    .InsertPagingAnchor(anchor)
                    .InsertPagingDirection(PagingDirection.FORWARD)
                    .InsertFields(["group_album.aid", "group_album.title", "group_album.user_id", "group_album.ADD_PHOTO_ALLOWED"]));
                response = await okApi.CallAsync<GroupAlbumsPageResponse>(GetAlbumsMethodName,
                    context.AccessPair, parameters, cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
                return new(albums.Values.ToArray(), false, "InvalidResponse");
            }
            catch (OkApiException)
            {
                return new(albums.Values.ToArray(), false, "ApiError");
            }
            catch (HttpRequestException)
            {
                return new(albums.Values.ToArray(), false, "TransportError");
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return new(albums.Values.ToArray(), false, "TransportTimeout");
            }

            if (response?.Albums is null)
                return new(albums.Values.ToArray(), false, "MissingAlbums");
            foreach (var album in response.Albums)
            {
                if (album is null || string.IsNullOrWhiteSpace(album.Id) || album.Title is null)
                    return new(albums.Values.ToArray(), false, "InvalidAlbum");
                albums.TryGetValue(album.Id, out var observed);
                if (!string.IsNullOrWhiteSpace(observed?.UserId) &&
                    !string.IsNullOrWhiteSpace(album.UserId) && observed.UserId != album.UserId)
                    return new(albums.Values.ToArray(), false, "ConflictingOwner");
                albums[album.Id] = new AlbumData
                {
                    Id = album.Id, Title = album.Title,
                    UserId = string.IsNullOrWhiteSpace(album.UserId) ? observed?.UserId : album.UserId,
                    Type = album.Type,
                    IsAddPhotoAllowed = album.Attributes?.Flags == "ap" && (observed?.IsAddPhotoAllowed ?? true)
                };
            }
            if (response.HasMore is null)
                return new(albums.Values.ToArray(), false, "MissingPagination");
            if (response.HasMore == false)
                return new(albums.Values.ToArray(), true, null);
            if (response.Albums.Length == 0)
                return new(albums.Values.ToArray(), false, "EmptyContinuation");
            if (string.IsNullOrWhiteSpace(response.Anchor) || !anchors.Add(response.Anchor))
                return new(albums.Values.ToArray(), false, "InvalidContinuation");
            anchor = response.Anchor;
        }
        return new(albums.Values.ToArray(), false, "ReadBudgetExceeded");
    }

    private const string OkClassName = "photos";

    private static readonly ICollection<string> DefaultFields =
        [
            "album.title",
            "album.aid",
            "album.user_id",
            "album.type",
            "album.ADD_PHOTO_ALLOWED",
            "group_album.title",
            "group_album.aid",
            "group_album.user_id",
            // У группового альбома OK нет поля type (GroupPhotoAlbumBeanFields): запрос group_album.type
            // роняет photos.getAlbums/getAlbumInfo для групп с PARAM-ошибкой. Видимость сверяем только
            // для страниц (album.type), поэтому для групп тип и не нужен.
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

        await okApi.CallAsync(
            SetAlbumMainPhotoMethodName,
            context.AccessPair,
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
        
        var response = await okApi.CallAsync<string>(CreateAlbumMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
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
        
        await okApi.CallAsync<string>(DeleteAlbumMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
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

        var response = await okApi.CallAsync<AlbumsResponse>(GetAlbumsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        if (response == null) return null;

        return new AnchorResponse<AlbumData>()
        {
            Anchor = response.Anchor,
            Results = response.Albums.Select(item => new AlbumData
            {
                Id = item.Id,
                Title = item.Title,
                UserId = item.UserId,
                Type = item.Type,
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
        CancellationToken cancellationToken = default,
        string? type = null)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertTitle(title)
            .InsertDescription(description);

        // Тип (видимость) альбома меняется только если он задан явно: PUBLIC/PRIVATE/FRIENDS и т. п.
        // OK применяет photos.editAlbum type к альбому целиком; без параметра видимость не трогается.
        if (!string.IsNullOrWhiteSpace(type))
            parameters = parameters.InsertCustomParameter("type", type);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        await okApi.CallAsync(EditAlbumMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
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
        
        var response = await okApi.CallAsync<AlbumResponse>(GetAlbumInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        if (response?.Album == null) return null;

        return new AlbumData
        {
            Id = response.Album.Id,
            Title = response.Album.Title,
            UserId = response.Album.UserId,
            Type = response.Album.Type
        };
    }
}
