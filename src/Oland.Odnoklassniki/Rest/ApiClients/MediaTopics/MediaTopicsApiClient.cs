using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Application.Services;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Response;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Enums;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Models;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Responses;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics;

/// <inheritdoc />
public class MediaTopicsApiClient(IOkApiClientCore okApi, IMediaService mediaService) : IMediaTopicsApiClient
{
    private const string OkClassName = "mediatopic";

    private const string PostMethodName = $"{OkClassName}.post";

    /// <inheritdoc />
    public async Task<string?> PostAsync(
        Action<MediaCollectionBuilder> buildAttachment,
        MediaTopicPostOptions options,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        MediaTopicOwnerType resolvedType;
        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                resolvedType = options.Type == MediaTopicOwnerType.USER
                    ? MediaTopicOwnerType.GROUP_THEME
                    : options.Type;
                break;
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                resolvedType = options.Type;
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        parameters = parameters.InsertMediaTopicType(resolvedType);

        if (options.OnBehalfOfGroup)
        {
            parameters = parameters.InsertOnBehalfOfGroup(true);
        }

        if (options.PublishAt.HasValue)
        {
            parameters = parameters.InsertPublishAt(options.PublishAt.Value);
        }

        if (options.DisableComments)
        {
            parameters = parameters.InsertDisableComments(true);
        }

        if (options.HiddenPost)
        {
            parameters = parameters.InsertHiddenPost(true);
        }

        if (!options.TextLinkPreview)
        {
            parameters = parameters.InsertTextLinkPreview(false);
        }

        if (options.SetStatus)
        {
            parameters = parameters.InsertSetStatus(true);
        }

        if (!string.IsNullOrEmpty(options.Locale))
        {
            parameters = parameters.InsertLocale(options.Locale);
        }

        var attachment = mediaService.CreateAndSerialize(buildAttachment, true);
        parameters = parameters.InsertAttachment(attachment);

        var response = await okApi.CallAsync<string>(PostMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.Trim('"');
    }

    private const string EditMethodName = $"{OkClassName}.edit";

    /// <inheritdoc />
    public async Task<bool> EditAsync(
        string topicId,
        Action<MediaCollectionBuilder> buildAttachment,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertTopicId(topicId);

        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        var attachment = mediaService.CreateAndSerialize(buildAttachment, true);
        parameters = parameters.InsertAttachment(attachment);

        var response = await okApi.CallAsync<CompletionStatusResponse>(EditMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.Success ?? false;
    }

    private const string DeleteTopicMethodName = $"{OkClassName}.deleteTopic";

    /// <inheritdoc />
    public async Task<bool> DeleteTopicAsync(
        string topicId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertTopicId(topicId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext),
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }

        var response = await okApi.CallAsync<DeleteTopicResponse>(DeleteTopicMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.Success ?? false;
    }

    private const string GetByIdsMethodName = $"{OkClassName}.getByIds";

    /// <inheritdoc />
    public async Task<ICollection<TDto>> GetByIdsAsync<TDto>(
        ICollection<string> topicIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        if (topicIds.Count == 0)
        {
            return [];
        }

        fields ??= [MediaTopicBeanFields.Id];

        var parameters = new RestParameters()
            .InsertCustomParameter("topic_ids", string.Join(',', topicIds))
            .InsertFields(fields.ToArray());

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext or GroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext),
                    nameof(GroupRequestContext));
        }

        var response = await okApi.CallAsync<MediaTopicsResponse<TDto>>(GetByIdsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.MediaTopics ?? [];
    }

    private const string GetRepublishedTopicMethodName = $"{OkClassName}.getRepublishedTopic";

    /// <inheritdoc />
    public async Task<string?> GetRepublishedTopicAsync(
        string topicId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertTopicId(topicId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<RepublishedTopicResponse>(GetRepublishedTopicMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.TopicId;
    }

    private const string GetPollVotersMethodName = $"{OkClassName}.getPollAnswerVoters";

    /// <inheritdoc />
    public AnchorNavigator<PollVoterDto> GetPollVotersNavigator(
        string topicId,
        int answerIndex,
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<PollVoterDto>(
            cfg => GetPollVotersAsync(topicId, answerIndex, context, cfg, cancellationToken),
            anchorConfiguration);
    }

    private async Task<AnchorResponse<PollVoterDto>> GetPollVotersAsync(
        string topicId,
        int answerIndex,
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertTopicId(topicId)
            .InsertCustomParameter("answer_idx", answerIndex)
            .InsertAnchor(anchorConfiguration.Anchor)
            .InsertCount(anchorConfiguration.Count);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<PollVotersResponse>(GetPollVotersMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<PollVoterDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Entities?.Users ?? []
        };
    }
}
