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
}