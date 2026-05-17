using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Friends.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Friends.Responses;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Friends;

/// <summary>
/// Клиент для взаимодействия с API друзей социальной сети Одноклассники (OK.ru).
/// </summary>
public class FriendsApiClient(IOkApiClientCore okApi) : IFriendsApiClient
{
    private const string OkClassName = "friends";
    private const string GetMethodName = $"{OkClassName}.get";

    /// <inheritdoc />
    public async Task<ICollection<string>> GetUserFriendsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        parameters = context switch
        {
            FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext => context.Apply(parameters),
            _ => throw new UnexpectedRequestContext(context, nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext))
        };

        var result = await okApi.CallAsync<ICollection<string>>(
            GetMethodName,
            context.AccessPair,
            parameters,
            cancellationToken: cancellationToken);

        return result ?? [];
    }

    private const string GetAppUsersMethodName = $"{OkClassName}.getAppUsers";

    /// <inheritdoc />
    public async Task<ICollection<string>?> GetAppUsersAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<FriendUidsResponse>(
            GetAppUsersMethodName, context.AccessPair, cancellationToken: cancellationToken);
        return response?.Uids;
    }

    private const string GetOnlineMethodName = $"{OkClassName}.getOnline";

    /// <inheritdoc />
    public async Task<ICollection<string>?> GetOnlineAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        switch (context)
        {
            case FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<string>>(
            GetOnlineMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetMutualFriendsMethodName = $"{OkClassName}.getMutualFriends";

    /// <inheritdoc />
    public async Task<ICollection<string>?> GetMutualFriendsAsync(
        string targetUid,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("target_id", targetUid);

        switch (context)
        {
            case FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<string>>(
            GetMutualFriendsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetByDevicesMethodName = $"{OkClassName}.getByDevices";

    /// <inheritdoc />
    public async Task<ICollection<string>?> GetByDevicesAsync(
        IEnumerable<string> devices,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("devices", string.Join(',', devices));

        switch (context)
        {
            case FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<string>>(
            GetByDevicesMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetBirthdaysMethodName = $"{OkClassName}.getBirthdays";

    /// <inheritdoc />
    public async Task<ICollection<UserBirthdayDto>?> GetBirthdaysAsync(
        IRequestContext context,
        bool future = false,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("future", future);

        switch (context)
        {
            case FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<UserBirthdayDto>>(
            GetBirthdaysMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetSuggestionsMethodName = $"{OkClassName}.getSuggestions";

    /// <inheritdoc />
    public async Task<ICollection<string>?> GetSuggestionsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        switch (context)
        {
            case FriendRequestContext or MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(FriendRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<FriendSuggestionsResponse>(
            GetSuggestionsMethodName, context.AccessPair, cancellationToken: cancellationToken);

        return response?.Users?.Select(u => u.Uid).Where(uid => uid != null).Cast<string>().ToList();
    }
}
