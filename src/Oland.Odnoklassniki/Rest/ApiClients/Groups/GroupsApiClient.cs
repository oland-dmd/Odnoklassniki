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
        
        fields ??= [GroupBeanFields.Name, GroupBeanFields.Uid, GroupBeanFields.AddPhotoAlbumAllowed];

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