using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2.Responses;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;

/// <summary>
/// Клиент для взаимодействия с API фотографий (версия 2) социальной сети Одноклассники (OK.ru).
/// </summary>
public class PhotosV2ApiClient(IOkApiClientCore okApi) : IPhotosV2ApiClient
{
    private const string OkClassName = "photosV2";
    private const string GetUploadUrlMethodName = $"{OkClassName}.getUploadUrl";
    private const string CommitMethodName = $"{OkClassName}.commit";

    /// <inheritdoc />
    public async Task<UploadUrlData> GetUploadUrlAsync(IRequestContext context,
        string? albumId = "",
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCount(1);

        switch (context)
        {
            case GroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        if (!string.IsNullOrWhiteSpace(albumId))
        {
            parameters = parameters.InsertAlbumId(albumId);
        }

        var response = await okApi.CallAsync<UploadPhotoResponse>(
            GetUploadUrlMethodName,
            context.AccessPair.AccessToken,
            context.AccessPair.SessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);

        return new UploadUrlData
        {
            PhotoIds = response.PhotoIds,
            UploadUrl = response.UploadUrl
        };
    }

    /// <inheritdoc />
    public async Task<ICollection<CommitPhotoData>> CommitAsync(string comment,
        string photoId,
        string token,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("comment", comment)
            .InsertCustomParameter("photo_id", photoId)
            .InsertCustomParameter("token", token);
        
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<CommitResponse>(
            CommitMethodName,
            context.AccessPair.AccessToken,
            context.AccessPair.SessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);

        return response.Photos.Select(item => new CommitPhotoData
        {
            Id = item.Id,
            Status = item.Status
        }).ToArray();
    }
}