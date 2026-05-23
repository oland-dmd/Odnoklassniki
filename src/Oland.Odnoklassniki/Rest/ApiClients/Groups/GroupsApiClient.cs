using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Extensions;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups;

/// <summary>
/// Клиент для работы с группами в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class GroupsApiClient(IOkApiClientCore okApi, MainAccountRequestContext mainContext) : IGroupsApiClient
{
    private const string OkClassName = "group";

    private const string GetInfoMethodName = $"{OkClassName}.getInfo";

    /// <inheritdoc />
    public async Task<ICollection<T>?> GetGroupsInfoAsync<T>(ICollection<string> groupIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where T: BaseOkDto
    {
        if (groupIds.Count == 0)
            return [];
        
        fields ??= [GroupBeanFields.Name, GroupBeanFields.Uid, GroupBeanFields.AddPhotoAlbumAllowed, GroupBeanFields.AddThemeAllowed];

        var parameters = new RestParameters()
            .InsertGroups(groupIds)
            .InsertFields(fields.ToArray());

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<T[]>(
            GetInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response;
    }

    private const string GetUserGroupsByIdsMethodName = $"{OkClassName}.getUserGroupsByIds";

    /// <inheritdoc />
    public async Task<ICollection<GroupUserInfoDto>?> GetUserGroupsInfoByIdsAsync(
        GroupId groupId,
        ICollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        if (userIds == null || userIds.Count == 0)
            throw new ArgumentException("User IDs collection cannot be empty", nameof(userIds));
        
        var parameters = new RestParameters()
            .InsertUsers(userIds)
            .InsertRootGroupId(groupId.Value);

        var response = await okApi.CallAsync<ICollection<GroupUserInfoResponse>>(
            GetUserGroupsByIdsMethodName, mainContext.AccessPair, parameters: parameters, cancellationToken: cancellationToken);

        return response?.Select(item => new GroupUserInfoDto
        {
            UserId = item.UserId,
            Status = Enum.Parse<GroupStatus>(item.Status)
        }).ToArray();
    }

    /// <inheritdoc />
    public AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserGroupDto>(
            nextPageConfiguration => GetUserGroupsV2CoreAsync(context, nextPageConfiguration, cancellationToken),
            anchorConfiguration);
    }

    private const string GetMembersMethodName = $"{OkClassName}.getMembers";

    /// <inheritdoc />
    public AnchorNavigator<GroupMemberDto> GetMembersNavigator(
        IRequestContext context,
        AnchorConfiguration cfg,
        GroupMemberStatusFilter? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<GroupMemberDto>(
            nextCfg => GetMembersCoreAsync(context, nextCfg, statusFilter, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<GroupMemberDto>> GetMembersCoreAsync(
        IRequestContext context,
        AnchorConfiguration cfg,
        GroupMemberStatusFilter? statusFilter,
        CancellationToken cancellationToken)
    {
        // API использует uid вместо gid для идентификатора группы в этом методе
        var groupId = context switch
        {
            GroupRequestContext g => g.GroupId.Value,
            MainGroupRequestContext mg => mg.GroupId.Value,
            _ => throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainGroupRequestContext))
        };

        var parameters = new RestParameters()
            .InsertUserId(groupId)
            .InsertAnchor(cfg.Anchor)
            .InsertCount(cfg.Count)
            .InsertDirection(cfg.Direction);

        if (statusFilter.HasValue)
            parameters = parameters.InsertStatuses([statusFilter.Value.ToString()]);

        var response = await okApi.CallAsync<GroupMembersResponse>(
            GetMembersMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<GroupMemberDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Members?.ToArray() ?? []
        };
    }

    private const string GetCountersMethodName = $"{OkClassName}.getCounters";

    /// <inheritdoc />
    public async Task<GroupCountersDto?> GetCountersAsync(
        IRequestContext context,
        IEnumerable<string>? counterTypes = null,
        CancellationToken cancellationToken = default)
    {
        var groupId = context switch
        {
            GroupRequestContext g => g.GroupId.Value,
            MainGroupRequestContext mg => mg.GroupId.Value,
            _ => throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainGroupRequestContext))
        };

        var parameters = new RestParameters()
            .InsertRootGroupId(groupId);

        if (counterTypes != null)
            parameters = parameters.InsertCounterTypes(counterTypes);

        var response = await okApi.CallAsync<GroupCountersResponse>(
            GetCountersMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Counters;
    }

    private const string GetStatOverviewMethodName = $"{OkClassName}.getStatOverview";

    /// <inheritdoc />
    public async Task<GroupStatOverviewDto?> GetStatOverviewAsync(
        IRequestContext context,
        GroupStatPeriod? period = null,
        long? startTime = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (context is not MainGroupRequestContext mgrc)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertGroupId(mgrc.GroupId.Value)
            .InsertFields(fields?.ToList());

        if (period.HasValue)
            parameters = parameters.InsertCustomParameter("period", period.Value.ToString());

        if (startTime.HasValue)
            parameters = parameters.InsertStartTime(startTime.Value);

        parameters = context.Apply(parameters);

        return await okApi.CallAsync<GroupStatOverviewDto>(
            GetStatOverviewMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetStatPeopleMethodName = $"{OkClassName}.getStatPeople";

    /// <inheritdoc />
    public async Task<GroupStatPeopleDto?> GetStatPeopleAsync(
        IRequestContext context,
        string? demoType = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (context is not MainGroupRequestContext mgrc)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertGroupId(mgrc.GroupId.Value)
            .InsertFields(fields?.ToList());

        if (!string.IsNullOrEmpty(demoType))
            parameters = parameters.InsertCustomParameter("demo_type", demoType);

        parameters = context.Apply(parameters);

        return await okApi.CallAsync<GroupStatPeopleDto>(
            GetStatPeopleMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetStatTopicMethodName = $"{OkClassName}.getStatTopic";

    /// <inheritdoc />
    public async Task<GroupStatTopicDto?> GetStatTopicAsync(
        string topicId,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (context is not MainGroupRequestContext)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertTopicId(topicId)
            .InsertFields(fields?.ToList());

        parameters = context.Apply(parameters);

        var response = await okApi.CallAsync<GroupStatTopicResponse>(
            GetStatTopicMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Topic;
    }

    private const string GetStatTopicsMethodName = $"{OkClassName}.getStatTopics";

    /// <inheritdoc />
    public AnchorNavigator<GroupStatTopicDto> GetStatTopicsNavigator(
        long startTime,
        long endTime,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<GroupStatTopicDto>(
            nextCfg => GetStatTopicsCoreAsync(startTime, endTime, context, nextCfg, fields, cancellationToken),
            cfg);
    }

    private async Task<AnchorResponse<GroupStatTopicDto>> GetStatTopicsCoreAsync(
        long startTime,
        long endTime,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields,
        CancellationToken cancellationToken)
    {
        if (context is not MainGroupRequestContext mgrc)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertGroupId(mgrc.GroupId.Value)
            .InsertStartTime(startTime)
            .InsertEndTime(endTime)
            .InsertAnchor(cfg.Anchor)
            .InsertCount(cfg.Count)
            .InsertFields(fields?.ToList());

        parameters = context.Apply(parameters);

        var response = await okApi.CallAsync<GroupStatTopicsResponse>(
            GetStatTopicsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<GroupStatTopicDto>
        {
            Anchor = response?.Anchor ?? string.Empty,
            HasMore = response?.HasMore ?? false,
            TotalCount = response?.TotalCount ?? 0,
            Results = response?.Topics?.ToArray() ?? []
        };
    }

    private const string GetStatTrendsMethodName = $"{OkClassName}.getStatTrends";

    /// <inheritdoc />
    public async Task<GroupStatTrendsDto?> GetStatTrendsAsync(
        long startTime,
        long endTime,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (context is not MainGroupRequestContext mgrc)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertGroupId(mgrc.GroupId.Value)
            .InsertStartTime(startTime)
            .InsertEndTime(endTime)
            .InsertFields(fields?.ToList());

        parameters = context.Apply(parameters);

        return await okApi.CallAsync<GroupStatTrendsDto>(
            GetStatTrendsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string PinGroupFeedMethodName = $"{OkClassName}.pinGroupFeed";

    /// <inheritdoc />
    public async Task<bool> PinGroupFeedAsync(
        string pinId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        // API принимает только pin_id, gid не требуется — токен уже идентифицирует группу
        if (context is not (GroupRequestContext or MainGroupRequestContext))
            throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertPinId(pinId);

        var response = await okApi.CallAsync<PinGroupFeedResponse>(
            PinGroupFeedMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    private const string SetMainPhotoMethodName = $"{OkClassName}.setMainPhoto";

    /// <inheritdoc />
    public async Task<bool> SetMainPhotoAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not MainGroupRequestContext mgrc)
            throw new UnexpectedRequestContext(context, nameof(MainGroupRequestContext));

        var parameters = new RestParameters()
            .InsertRootGroupId(mgrc.GroupId.Value)
            .InsertPhotoId(photoId);

        parameters = context.Apply(parameters);

        return await okApi.CallAsync<bool>(
            SetMainPhotoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string IsMessagesAllowedMethodName = $"{OkClassName}.isMessagesAllowed";

    /// <inheritdoc />
    public async Task<bool> IsMessagesAllowedAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        switch (context)
        {
            case GroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            case MainGroupRequestContext mg:
                parameters = parameters.InsertGroupId(mg.GroupId.Value);
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext), nameof(MainGroupRequestContext));
        }

        var response = await okApi.CallAsync<IsMessagesAllowedResponse>(
            IsMessagesAllowedMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Allowed ?? false;
    }

    private const string GetUserGroupsV2MethodName = $"{OkClassName}.getUserGroupsV2";

    private async Task<AnchorResponse<UserGroupDto>> GetUserGroupsV2CoreAsync(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertAnchor(anchorConfiguration.Anchor)
            .InsertDirection(anchorConfiguration.Direction)
            .InsertCount(anchorConfiguration.Count);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<UserGroupsResponse>(
            GetUserGroupsV2MethodName,
            context.AccessPair,
            parameters,
            cancellationToken: cancellationToken);

        return new AnchorResponse<UserGroupDto>()
        {
            Anchor = response.Anchor,
            Results = response.Response?.Select(groupResponse => new UserGroupDto()
            {
                GroupId = groupResponse.GroupId,
                UserId = groupResponse.UserId
            }).ToArray(),
            HasMore = response.Response?.Count != 0
        };
    }
}