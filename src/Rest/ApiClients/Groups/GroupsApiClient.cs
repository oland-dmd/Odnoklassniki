using Odnoklassniki.Enums;
using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Odnoklassniki.Rest.ApiClients.Groups.Responses;
using Odnoklassniki.Rest.ApiClients.Groups.Extensions;

namespace Odnoklassniki.Rest.ApiClients.Groups;

/// <summary>
/// Клиент для работы с группами в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public class GroupsApiClient(IOkApiClientCore okApi) : IGroupsApiClient
{
    private const string OkClassName = "group";

    #region Get Group Info

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public Task<ICollection<GroupInfoDto>?> GetGroupsInfoAsync(
        ICollection<string> groupIds,
        CancellationToken cancellationToken = default)
    {
        return GetGroupsInfoWithDefaultCredentialsAsync(groupIds, cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public Task<ICollection<GroupInfoDto>?> GetGroupsInfoAsync(
        ICollection<string> groupIds,
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default)
    {
        return GetGroupsInfoWithExplicitCredentialsAsync(groupIds, accessToken, sessionSecretKey, cancellationToken);
    }

    #endregion

    #region Get User Groups Info

    // Этот метод работает ТОЛЬКО с основными кредами по дизайну API
    // Перегрузку с токенами не добавляем, чтобы не вводить в заблуждение

    /// <inheritdoc />
    public Task<ICollection<GroupUserInfoDto>?> GetUserGroupsInfoByIdsAsync(
        string groupId,
        ICollection<string> userIds,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be empty", nameof(groupId));
        if (userIds == null || userIds.Count == 0)
            throw new ArgumentException("User IDs collection cannot be empty", nameof(userIds));
        
        return GetUserGroupsInfoByIdsCoreAsync(groupId, userIds, cancellationToken);
    }

    #endregion

    #region Get User Groups (V2 with Pagination)

    // === Для основного аккаунта ===

    /// <inheritdoc />
    public AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(string anchor,
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken = default)
    {
        return GetUserGroupsWithDefaultCredentialsAsync(direction, count, cancellationToken);
    }

    // === С явными токенами ===

    /// <inheritdoc />
    public AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(string accessToken,
        string sessionSecretKey,
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken = default)
    {
        return new AnchorNavigator<UserGroupDto>(anchor => GetUserGroupsV2CoreAsync(accessToken, sessionSecretKey, anchor, direction, count, cancellationToken));
    }

    #endregion

    #region Internal Helpers: Default Credentials

    private const string GetInfoMethodName = $"{OkClassName}.getInfo";

    private async Task<ICollection<GroupInfoDto>?> GetGroupsInfoWithDefaultCredentialsAsync(
        ICollection<string> groupIds,
        CancellationToken cancellationToken)
    {
        return await GetGroupsInfoCoreAsync(groupIds, "", "", cancellationToken);
    }

    private const string GetUserGroupsV2MethodName = $"{OkClassName}.getUserGroupsV2";

    private AnchorNavigator<UserGroupDto> GetUserGroupsWithDefaultCredentialsAsync(
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken)
    {
        return new AnchorNavigator<UserGroupDto>(anchor => GetUserGroupsV2CoreAsync("", "", anchor, direction, count, cancellationToken));
    }

    #endregion

    #region Internal Helpers: Explicit Credentials

    private async Task<ICollection<GroupInfoDto>?> GetGroupsInfoWithExplicitCredentialsAsync(
        ICollection<string> groupIds,
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken)
    {
        return await GetGroupsInfoCoreAsync(groupIds, accessToken, sessionSecretKey, cancellationToken);
    }

    private AnchorNavigator<UserGroupDto> GetUserGroupsWithExplicitCredentialsAsync(
        string accessToken,
        string sessionSecretKey,
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken)
    {
        return new AnchorNavigator<UserGroupDto>(anchor => GetUserGroupsV2CoreAsync(accessToken, sessionSecretKey, anchor, direction, count, cancellationToken));
    }

    #endregion

    #region Core Implementation (Single Source of Truth)

    private async Task<ICollection<GroupInfoDto>?> GetGroupsInfoCoreAsync(
        ICollection<string>? groupIds,
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken)
    {
        if (groupIds == null || groupIds.Count == 0)
            return [];

        var parameters = new RestParameters()
            .InsertGroups(groupIds)
            .InsertFields("name", "uid", "ADD_PHOTOALBUM_ALLOWED");

        var response = await okApi.CallAsync<GroupInfoResponse[]>(
            GetInfoMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return response?.Select(r => new GroupInfoDto { Id = r.Id, Name = r.Name, AddAlbumAllowed = r.Attributes?.Flags.Contains("ap") ?? false }).ToArray();
    }

    private const string GetUserGroupsByIdsMethodName = $"{OkClassName}.getUserGroupsByIds";

    private async Task<ICollection<GroupUserInfoDto>?> GetUserGroupsInfoByIdsCoreAsync(
        string groupId,
        ICollection<string> userIds,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertUsers(userIds)
            .InsertRootGroupId(groupId);

        var response = await okApi.CallAsync<ICollection<GroupUserInfoResponse>>(
            GetUserGroupsByIdsMethodName, parameters: parameters, cancellationToken: cancellationToken);

        return response?.Select(item => new GroupUserInfoDto
        {
            UserId = item.UserId,
            Status = Enum.Parse<GroupStatus>(item.Status)
        }).ToArray();
    }

    private async Task<AnchorResponse<UserGroupDto>> GetUserGroupsV2CoreAsync(
        string accessToken,
        string sessionSecretKey,
        string anchor,
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertAnchor(anchor)
            .InsertDirection(direction)
            .InsertCount(count);

        var response = await okApi.CallAsync<UserGroupsResponse>(
            GetUserGroupsV2MethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

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

    #endregion
}