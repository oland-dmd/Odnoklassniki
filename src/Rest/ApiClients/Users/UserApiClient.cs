using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.ApiClients.Users.Datas;
using Odnoklassniki.Rest.ApiClients.Users.Responses;

namespace Odnoklassniki.Rest.ApiClients.Users;

public class UserApiClient(IOkApiClientCore okApi) : IUserApiClient
{
    private const string OkClassName = "users";

    private const string GetLoggedInUserMethodName = $"{OkClassName}.getLoggedInUser";

    public async Task<string?> GetLoggedInUserAsync(string accessToken, string sessionSecretKey, CancellationToken cancellationToken = default)
    {
        var result = await okApi.CallAsync<string>(GetLoggedInUserMethodName, accessToken, sessionSecretKey, cancellationToken: cancellationToken);

        return result?.Trim('"');
    }

    private const string GetCurrentUserMethodName = $"{OkClassName}.getCurrentUser";

    public async Task<UserData> GetCurrentUserAsync(string accessToken, string sessionSecretKey, CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFields("first_name", "last_name", "uid");
        
        var response = await okApi.CallAsync<UserInfoResponse>(GetCurrentUserMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        var result = new UserData
        {
            FirstName = response.FirstName,
            LastName = response.LastName,
            UID = response.UID
        };

        return result;
    }
}
