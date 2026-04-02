using Odnoklassniki.Exceptions;
using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.ApiClients.Users.Datas;
using Odnoklassniki.Rest.ApiClients.Users.Responses;
using Odnoklassniki.Rest.RequestContexts;

namespace Odnoklassniki.Rest.ApiClients.Users;

/// <inheritdoc />
public class UserApiClient(IOkApiClientCore okApi) : IUserApiClient
{
    private const string OkClassName = "users";

    private const string GetLoggedInUserMethodName = $"{OkClassName}.getLoggedInUser";

    /// <inheritdoc />
    public async Task<string?> GetLoggedInUserAsync(IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        var result = await okApi.CallAsync<string>(GetLoggedInUserMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, cancellationToken: cancellationToken);

        return result?.Trim('"');
    }

    private const string GetCurrentUserMethodName = $"{OkClassName}.getCurrentUser";

    /// <inheritdoc />
    public async Task<UserData> GetCurrentUserAsync(IRequestContext context, CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFields("first_name", "last_name", "uid");
        
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<UserInfoResponse>(GetCurrentUserMethodName, context.AccessPair.AccessToken, context.AccessPair.SessionSecretKey, parameters, cancellationToken: cancellationToken);

        var result = new UserData
        {
            FirstName = response.FirstName,
            LastName = response.LastName,
            UID = response.UID
        };

        return result;
    }
}
