using Microsoft.Extensions.Logging;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Enums;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.Extensions;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions;

/// <summary>
/// Клиент для взаимодействия с API обсуждений (discussions) социальной сети «Одноклассники».
/// </summary>
public class DiscussionsApiClient(IOkApiClientCore okApi, ILogger<DiscussionsApiClient> logger) : IDiscussionsApiClient
{
    private const string OkClassName = "discussions";
    private const string GetListMethodName = $"{OkClassName}.getList";
    private const string GetCommentsMethodName = $"{OkClassName}.getComments";

    // === PUBLIC API (из интерфейса) ===

    /// <inheritdoc />
    public Task<ICollection<DiscussionData>> GetGroupListAsync(ExplicitTokenRequestContext context,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return GetListInternalAsync(context, Category.GROUP, count, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ICollection<DiscussionData>> GetUserListAsync(ExplicitTokenRequestContext context,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return GetListInternalAsync(context, Category.MY, count, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ICollection<CommentData>> GetCommentsAsync(ExplicitTokenRequestContext context,
        string discussionId,
        string discussionType,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "comment.AUTHOR_ID", "comment.AUTHOR_NAME", "comment.AUTHOR_REF",
                "comment.REPLY_TO_ID", "comment.REPLY_TO_NAME", "comment.REPLY_TO_COMMENT_ID",
                "comment.CREATED_MS", "comment.DATE", "comment.ID", "comment.TEXT", "comment.TYPE")
            .InsertCount(count)
            .InsertCustomParameter("discussionType", discussionType)
            .InsertCustomParameter("discussionId", discussionId)
            .InsertCustomParameter("mark_as_read", true)
            .InsertCustomParameter("includeRemoved", true);
        
        var response = await okApi.CallAsync<CommentResponse>(
            GetCommentsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        // OK может вернуть ответ без поля "comments" (обсуждение без новых комментариев / уже прочитаны) —
        // тогда Comments == null. Защищаемся, возвращая пустую коллекцию вместо NRE.
        return (response?.Comments ?? []).Select(item => new CommentData
        {
            AuthorId = item.AuthorId,
            Timestamp = item.Timestamp,
            ID = item.ID,
            ReplyToCommentId = item.ReplyToCommentId,
            ReplyToId = item.ReplyToId,
            ReplyToName = item.ReplyToName,
            Text = item.Text,
            Type = item.Type,
            AuthorName = item.AuthorName,
            AuthorRef = item.AuthorRef
        }).ToArray();
    }

    // === НОВЫЕ МЕТОДЫ (фаза 7) ===

    private const string GetDiscussionMethodName = $"{OkClassName}.get";

    /// <inheritdoc />
    public AnchorNavigator<TDto> GetListNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration cfg,
        ICollection<ApiDiscussionType>? types = null,
        ApiDiscussionTypeFilter? category = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        return new AnchorNavigator<TDto>(
            nextCfg => GetListCoreAsync<TDto>(context, nextCfg, types, category, fields, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<TDto>> GetListCoreAsync<TDto>(
        IRequestContext context,
        AnchorConfiguration cfg,
        ICollection<ApiDiscussionType>? types,
        ApiDiscussionTypeFilter? category,
        IEnumerable<string>? fields,
        CancellationToken cancellationToken) where TDto : BaseOkDto
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertAnchor(cfg.Anchor)
            .InsertCount(cfg.Count)
            .InsertDirection(cfg.Direction)
            .InsertFields(fields?.ToArray());

        if (types?.Count > 0)
            parameters = parameters.InsertCustomParameter("types", string.Join(",", types));

        if (category.HasValue)
            parameters = parameters.InsertCustomParameter("category", category.Value.ToString());

        var response = await okApi.CallAsync<DiscussionListResponse<TDto>>(
            GetListMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<TDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = !string.IsNullOrEmpty(response?.Anchor),
            TotalCount = 0,
            Results = response?.Discussions?.ToArray() ?? []
        };
    }

    /// <inheritdoc />
    public async Task<TDto?> GetAsync<TDto>(
        string discussionId,
        string discussionType,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertDiscussionId(discussionId, discussionType)
            .InsertFields(fields?.ToArray());

        var response = await okApi.CallAsync<DiscussionGetResponse<TDto>>(
            GetDiscussionMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Discussion;
    }

    private const string GetCommentMethodName = $"{OkClassName}.getComment";

    /// <inheritdoc />
    public async Task<CommentDetailData?> GetCommentAsync(
        string discussionId,
        string discussionType,
        string commentId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertDiscussionId(discussionId, discussionType)
            .InsertCommentId(commentId);

        var response = await okApi.CallAsync<CommentGetResponse>(
            GetCommentMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Comment;
    }

    private const string GetDiscussionCommentsMethodName = $"{OkClassName}.getDiscussionComments";

    /// <inheritdoc />
    public async Task<ICollection<CommentDetailData>?> GetDiscussionCommentsAsync(
        string discussionId,
        string discussionType,
        IRequestContext context,
        int offset = 0,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertEntityId(discussionId, discussionType)
            .InsertOffset(offset)
            .InsertCount(count);

        var response = await okApi.CallAsync<DiscussionCommentsResponse>(
            GetDiscussionCommentsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Comments;
    }

    private const string GetDiscussionCommentsCountMethodName = $"{OkClassName}.getDiscussionCommentsCount";

    /// <inheritdoc />
    public async Task<int> GetDiscussionCommentsCountAsync(
        string discussionId,
        string discussionType,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertEntityId(discussionId, discussionType);

        var response = await okApi.CallAsync<DiscussionCommentsCountResponse>(
            GetDiscussionCommentsCountMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.CommentsCount ?? 0;
    }

    private const string GetDiscussionLikesMethodName = $"{OkClassName}.getDiscussionLikes";

    /// <inheritdoc />
    public AnchorNavigator<UserLikeDto> GetDiscussionLikesNavigator(
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserLikeDto>(
            nextCfg => GetDiscussionLikesCoreAsync(discussionId, discussionType, context, nextCfg, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<UserLikeDto>> GetDiscussionLikesCoreAsync(
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertDiscussionId(discussionId, discussionType)
            .InsertAnchor(cfg.Anchor)
            .InsertCount(cfg.Count)
            .InsertDirection(cfg.Direction);

        var response = await okApi.CallAsync<DiscussionLikesResponse>(
            GetDiscussionLikesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<UserLikeDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = !string.IsNullOrEmpty(response?.Anchor),
            TotalCount = 0,
            Results = response?.Users?.ToArray() ?? []
        };
    }

    private const string GetCommentLikesMethodName = $"{OkClassName}.getCommentLikes";

    /// <inheritdoc />
    public AnchorNavigator<UserLikeDto> GetCommentLikesNavigator(
        string commentId,
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserLikeDto>(
            nextCfg => GetCommentLikesCoreAsync(commentId, discussionId, discussionType, context, nextCfg, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<UserLikeDto>> GetCommentLikesCoreAsync(
        string commentId,
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertDiscussionId(discussionId, discussionType)
            .InsertCommentId(commentId)
            .InsertAnchor(cfg.Anchor)
            .InsertCount(cfg.Count)
            .InsertDirection(cfg.Direction);

        var response = await okApi.CallAsync<DiscussionLikesResponse>(
            GetCommentLikesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<UserLikeDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = !string.IsNullOrEmpty(response?.Anchor),
            TotalCount = 0,
            Results = response?.Users?.ToArray() ?? []
        };
    }

    private const string GetAttachedResourcesMethodName = $"{OkClassName}.getAttachedResources";

    /// <inheritdoc />
    public async Task<ICollection<AttachedResourceData>?> GetAttachedResourcesAsync(
        ICollection<string> attachIds,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertAttachIds(attachIds);

        var response = await okApi.CallAsync<AttachedResourcesResponse>(
            GetAttachedResourcesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Attachments;
    }

    private const string GetDiscussionsMethodName = $"{OkClassName}.getDiscussions";

    /// <inheritdoc />
    [Obsolete("Используйте GetListNavigator<TDto>. Метод deprecated в API OK.ru.")]
    public async Task<ICollection<DiscussionData>?> GetDiscussionsAsync(
        IRequestContext context,
        ICollection<string>? entityTypes = null,
        string? category = null,
        int offset = 0,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertOffset(offset)
            .InsertCount(count);

        if (entityTypes?.Count > 0)
            parameters = parameters.InsertCustomParameter("entityTypes", string.Join(",", entityTypes));

        if (!string.IsNullOrEmpty(category))
            parameters = parameters.InsertCustomParameter("category", category);

        var response = await okApi.CallAsync<DiscussionsLegacyResponse>(
            GetDiscussionsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Discussions?.Select(item => new DiscussionData
        {
            ID = item.EntityId ?? string.Empty,
            Type = item.EntityType ?? string.Empty,
            NewCommentCount = item.NewCommentsCount ?? 0,
            Title = item.SubjectLabel ?? string.Empty,
            AlbumId = string.Empty,
            GroupId = null
        }).ToArray();
    }

    private const string GetDiscussionsNewsMethodName = $"{OkClassName}.getDiscussionsNews";

    /// <inheritdoc />
    public async Task<ICollection<DiscussionNewsItemData>?> GetDiscussionsNewsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters();

        var response = await okApi.CallAsync<DiscussionsNewsResponse>(
            GetDiscussionsNewsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.News;
    }

    // === INTERNAL IMPLEMENTATION ===

    /// <summary>
    /// Внутренняя реализация получения списка обсуждений.
    /// Не выносится в интерфейс — используется только внутри класса.
    /// </summary>
    private async Task<ICollection<DiscussionData>> GetListInternalAsync(
        ExplicitTokenRequestContext context,
        Category category,
        int count,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "discussion.OBJECT_TYPE", "discussion.OBJECT_ID", "discussion.NEW_COMMENTS_COUNT",
                "group_album.AID", "group.UID", "album.AID", "discussion.TITLE")
            .InsertCount(count)
            .InsertCustomParameter("category", category);

        var response = await okApi.CallAsync<DiscussionResponse>(
            GetListMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return (response.Discussions ?? []).Select(item =>
            new DiscussionData
            {
                ID = item.ID,
                NewCommentCount = item.NewCommentCount,
                AlbumId = item.RefObjects.ExtractAlbumId(),
                GroupId = item.RefObjects.ExtractGroupId(),
                Type = item.Type,
                Title = item.Title
            }).ToArray();
    }
}