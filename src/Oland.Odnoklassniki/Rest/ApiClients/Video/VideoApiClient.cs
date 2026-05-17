using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Video.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Video.Responses;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Video;

/// <summary>
/// Клиент для работы с видеороликами в социальной сети Одноклассники (OK.ru).
/// Поддерживает получение URL загрузки, обновление и удаление видео, а также подписку на канал.
/// </summary>
public class VideoApiClient(IOkApiClientCore okApi) : IVideoApiClient
{
    private const string OkClassName = "video";

    private const string GetUploadUrlMethodName = $"{OkClassName}.getUploadUrl";

    /// <inheritdoc />
    public async Task<VideoUploadUrlData?> GetUploadUrlAsync(
        string fileName,
        long fileSize,
        IRequestContext context,
        string? attachmentType = null,
        string? channelId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFileName(fileName)
            .InsertFileSize(fileSize)
            .InsertChannelId(channelId ?? string.Empty);

        if (!string.IsNullOrEmpty(attachmentType))
            parameters = parameters.InsertCustomParameter("attachment_type", attachmentType);

        switch (context)
        {
            case MainGroupRequestContext mg:
                parameters = parameters.InsertGroupId(mg.GroupId.Value);
                break;
            case GroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext), nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<VideoUploadUrlData>(
            GetUploadUrlMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string UpdateMethodName = $"{OkClassName}.update";

    /// <inheritdoc />
    public async Task UpdateAsync(
        string videoId,
        IRequestContext context,
        string? title = null,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertVid(videoId);

        if (!string.IsNullOrEmpty(title))
            parameters = parameters.InsertTitle(title);

        if (!string.IsNullOrEmpty(description))
            parameters = parameters.InsertDescription(description);

        switch (context)
        {
            case MainGroupRequestContext mg:
                parameters = parameters.InsertGroupId(mg.GroupId.Value);
                break;
            case GroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext), nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        await okApi.CallAsync(
            UpdateMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string DeleteMethodName = $"{OkClassName}.delete";

    /// <inheritdoc />
    public async Task DeleteAsync(
        string videoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertVid(videoId);

        switch (context)
        {
            case MainGroupRequestContext mg:
                parameters = parameters.InsertGroupId(mg.GroupId.Value);
                break;
            case GroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext), nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        await okApi.CallAsync(
            DeleteMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string SubscribeMethodName = $"{OkClassName}.subscribe";

    /// <inheritdoc />
    public async Task<bool> SubscribeAsync(
        string channelId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context,
                nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertChannelId(channelId);

        var response = await okApi.CallAsync<VideoSubscribeResponse>(
            SubscribeMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }
}
