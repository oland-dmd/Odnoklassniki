using Odnoklassniki.Exceptions;
using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.AnchorNavigators;
using Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Odnoklassniki.Rest.ApiClients.Groups.Responses;
using Odnoklassniki.Rest.ApiClients.Groups.Extensions;
using Odnoklassniki.Rest.RequestContexts;
using Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.Rest.ApiClients.Groups;

/// <summary>
/// Клиент для работы с группами в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class GroupsApiClient(IOkApiClientCore okApi) : IGroupsApiClient
{
    private const string OkClassName = "group";

    private const string GetInfoMethodName = $"{OkClassName}.getInfo";

    /// <inheritdoc />
    public async Task<ICollection<GroupInfoDto>?> GetGroupsInfoAsync(ICollection<string> groupIds,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (groupIds.Count == 0)
            return [];

        var parameters = new RestParameters()
            .InsertGroups(groupIds)
            .InsertFields("name", "uid", "ADD_PHOTOALBUM_ALLOWED");

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        context.Deconstruct(out var accessToken, out var sessionSecretKey);
        
        var response = await okApi.CallAsync<GroupInfoResponse[]>(
            GetInfoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return response?.Select(r => new GroupInfoDto { Id = r.Id, Name = r.Name, AddAlbumAllowed = r.Attributes?.Flags.Contains("ap") ?? false }).ToArray();
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
            GetUserGroupsByIdsMethodName, parameters: parameters, cancellationToken: cancellationToken);

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

        context.Deconstruct(out var accessToken, out var sessionSecretKey);
        
        var response = await okApi.CallAsync<UserGroupsResponse>(
            GetUserGroupsV2MethodName,
            accessToken,
            sessionSecretKey,
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