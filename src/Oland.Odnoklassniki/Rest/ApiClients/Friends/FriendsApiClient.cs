using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
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
            _ => throw new UnexpectedRequestContext(context, nameof(FriendRequestContext),nameof(MainAccountRequestContext),nameof(ExplicitTokenRequestContext))
        };

        context.Deconstruct(out var accessToken, out var sessionSecretKey);
        
        var result = await okApi.CallAsync<ICollection<string>>(
            GetMethodName,
            accessToken,
            sessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);
        
        return result ?? [];
    }

}