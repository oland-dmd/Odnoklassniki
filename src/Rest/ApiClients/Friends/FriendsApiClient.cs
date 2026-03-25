using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;

namespace Odnoklassniki.Rest.ApiClients.Friends;

/// <summary>
/// Клиент для взаимодействия с API друзей социальной сети Одноклассники (OK.ru).
/// </summary>
public class FriendsApiClient(IOkApiClientCore okApi) : IFriendsApiClient
{
    private const string OkClassName = "friends";
    private const string GetMethodName = $"{OkClassName}.get";

    /// <inheritdoc />
    public Task<ICollection<string>> GetUserFriendsAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default)
    {
        // Пустой friendId = друзья текущего пользователя
        return GetFriendsInternalAsync(accessToken, sessionSecretKey, string.Empty, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ICollection<string>> GetFriendsOfaFriendAsync(
        string accessToken,
        string sessionSecretKey,
        string friendId,
        CancellationToken cancellationToken = default)
    {
        return GetFriendsInternalAsync(accessToken, sessionSecretKey, friendId, cancellationToken);
    }

    /// <summary>
    /// Внутренняя реализация получения списка друзей.
    /// Не выносится в интерфейс — инкапсулирует детали работы с параметрами.
    /// </summary>
    private async Task<ICollection<string>?> GetFriendsInternalAsync(
        string accessToken,
        string sessionSecretKey,
        string friendId,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertFriendId(friendId);

        return await okApi.CallAsync<ICollection<string>>(
            GetMethodName,
            accessToken,
            sessionSecretKey,
            parameters,
            cancellationToken: cancellationToken);
    }
}