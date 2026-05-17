using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Photo;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos;

/// <summary>
/// Клиент для работы с фотографиями в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class PhotosApiClient(IOkApiClientCore okApi) : IPhotosApiClient
{
    private const string OkClassName = "photos";
    /// <inheritdoc />
    public AnchorNavigator<PhotoData> GetPhotosNavigator(string albumId,
        IRequestContext context,
        AnchorConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<PhotoData>(nextAnchor => GetPhotosCoreAsync(albumId, context, nextAnchor, cancellationToken), configuration);
    }

    private const string GetPhotoInfoMethodName = $"{OkClassName}.getPhotoInfo";
    /// <inheritdoc />
    public async Task<PhotoData> GetPhotoInfoAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertFields(
                "photo.pic_max", "photo.id", "photo.text", "photo.user_Id",
                "group_photo.pic_max", "group_photo.id", "group_photo.text", "group_photo.user_Id");

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(FriendRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<PhotoResponse>(
            GetPhotoInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new PhotoData
        {
            Id = response.Photo.Id,
            PicMax = response.Photo.PicMax,
            Text = response.Photo.Text,
            UserId = response.Photo.UserId
        };
    }
    
    private const string GetPhotosMethodName = $"{OkClassName}.getPhotos";
    private async Task<AnchorResponse<PhotoData>> GetPhotosCoreAsync(
        string albumId,
        IRequestContext context,
        AnchorConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "photo.pic_max", "photo.id", "photo.text", "photo.user_Id",
                "group_photo.pic_max", "group_photo.id", "group_photo.text", "group_photo.user_Id")
            .InsertCount(configuration.Count)
            .InsertAnchor(configuration.Anchor);

        switch (context)
        {
            case GroupRequestContext or FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(FriendRequestContext));
        }

        if (!string.IsNullOrWhiteSpace(albumId))
            parameters = parameters.InsertAlbumId(albumId);
        
        var response = await okApi.CallAsync<PhotosResponse>(
            GetPhotosMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<PhotoData>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Photos.Select(item => new PhotoData
            {
                Id = item.Id,
                PicMax = item.PicMax,
                Text = item.Text,
                UserId = item.UserId,
            }).ToArray() ?? []
        };
    }

    private const string EditPhotosMethodName = $"{OkClassName}.editPhoto";

    /// <inheritdoc />
    public async Task<bool> EditPhotoAsync(string photoId, string description, IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertDescription(description);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(
            EditPhotosMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

    }

    private const string DeletePhotoMethodName = $"{OkClassName}.deletePhoto";

    /// <inheritdoc />
    public async Task DeletePhotoAsync(string photoId, IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        await okApi.CallAsync(
            DeletePhotoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetPhotosInfoMethodName = $"{OkClassName}.getInfo";
    /// <inheritdoc />
    public async Task<ICollection<TDto>?> GetPhotosInfoAsync<TDto>(
        ICollection<string> photoIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto
    {
        var parameters = new RestParameters()
            .InsertPhotoIds(photoIds)
            .InsertFields(fields?.ToList());

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(GroupRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<PhotosBatchInfoResponse<TDto>>(
            GetPhotosInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Photos;
    }

    private const string GetUserPhotosMethodName = $"{OkClassName}.getUserPhotos";
    /// <inheritdoc />
    public AnchorNavigator<TDto> GetUserPhotosNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto
    {
        return new AnchorNavigator<TDto>(
            nextCfg => GetUserPhotosCoreAsync<TDto>(null, context, nextCfg, fields, cancellationToken),
            cfg);
    }

    private const string GetUserAlbumPhotosMethodName = $"{OkClassName}.getUserAlbumPhotos";
    /// <inheritdoc />
    public AnchorNavigator<TDto> GetUserAlbumPhotosNavigator<TDto>(
        string albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto
    {
        return new AnchorNavigator<TDto>(
            nextCfg => GetUserPhotosCoreAsync<TDto>(albumId, context, nextCfg, fields, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<TDto>> GetUserPhotosCoreAsync<TDto>(
        string? albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields,
        CancellationToken cancellationToken)
        where TDto : BaseOkDto
    {
        var methodName = albumId is null ? GetUserPhotosMethodName : GetUserAlbumPhotosMethodName;

        var parameters = new RestParameters()
            .InsertFields(fields?.ToList())
            .InsertCount(cfg.Count)
            .InsertPagingAnchor(cfg.Anchor);

        if (albumId is not null)
            parameters = parameters.InsertAlbumId(albumId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<UserPhotosResponse<TDto>>(
            methodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<TDto>
        {
            Anchor = response?.PagingAnchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Photos?.ToArray() ?? []
        };
    }

    private const string AddPhotoLikeMethodName = $"{OkClassName}.addPhotoLike";
    /// <inheritdoc />
    public async Task<bool> AddPhotoLikeAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(
            AddPhotoLikeMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string AddAlbumLikeMethodName = $"{OkClassName}.addAlbumLike";
    /// <inheritdoc />
    public async Task<bool> AddAlbumLikeAsync(
        string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(
            AddAlbumLikeMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetPhotoLikesMethodName = $"{OkClassName}.getPhotoLikes";
    /// <inheritdoc />
    public AnchorNavigator<UserLikeDto> GetPhotoLikesNavigator(
        string photoId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserLikeDto>(
            nextCfg => GetPhotoLikesCoreAsync(photoId, context, nextCfg, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<UserLikeDto>> GetPhotoLikesCoreAsync(
        string photoId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertCount(cfg.Count)
            .InsertAnchor(cfg.Anchor);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(GroupRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<PhotoLikesResponse<UserLikeDto>>(
            GetPhotoLikesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<UserLikeDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Users?.ToArray() ?? []
        };
    }

    private const string GetAlbumLikesMethodName = $"{OkClassName}.getAlbumLikes";
    /// <inheritdoc />
    public AnchorNavigator<UserLikeDto> GetAlbumLikesNavigator(
        string albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserLikeDto>(
            nextCfg => GetAlbumLikesCoreAsync(albumId, context, nextCfg, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<UserLikeDto>> GetAlbumLikesCoreAsync(
        string albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertAlbumId(albumId)
            .InsertCount(cfg.Count)
            .InsertAnchor(cfg.Anchor);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(GroupRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<PhotoLikesResponse<UserLikeDto>>(
            GetAlbumLikesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<UserLikeDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Users?.ToArray() ?? []
        };
    }

    private const string GetPhotoMarksMethodName = $"{OkClassName}.getPhotoMarks";
    /// <inheritdoc />
    public async Task<ICollection<PhotoMarkDto>?> GetPhotoMarksAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(GroupRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<PhotoMarksResponse>(
            GetPhotoMarksMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Marks;
    }

    private const string GetTagsMethodName = $"{OkClassName}.getTags";
    /// <inheritdoc />
    public async Task<ICollection<PhotoTagDto>?> GetTagsAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or FriendRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext), nameof(GroupRequestContext), nameof(FriendRequestContext));
        }

        var response = await okApi.CallAsync<PhotoTagsResponse>(
            GetTagsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Entities?.Tags;
    }

    private const string DeleteTagsMethodName = $"{OkClassName}.deleteTags";
    /// <inheritdoc />
    public async Task<int> DeleteTagsAsync(
        string photoId,
        ICollection<string> tagIds,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertPhotoId(photoId)
            .InsertCustomParameter("tag_ids", string.Join(',', tagIds));

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<DeleteTagsResponse>(
            DeleteTagsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Count ?? 0;
    }
}