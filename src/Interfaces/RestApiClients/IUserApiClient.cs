using Odnoklassniki.Rest.ApiClients.Users.Datas;

namespace Odnoklassniki.Interfaces.RestApiClients;

public interface IUserApiClient
{
    /// <summary>
    /// Получает идентификатор авторизованного пользователя.
    /// </summary>
    /// <param name="accessToken">Токен доступа к API.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>UID пользователя или null, если не удалось получить.</returns>
    Task<string?> GetLoggedInUserAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает данные текущего пользователя.
    /// </summary>
    /// <param name="accessToken">Токен доступа к API.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект с данными пользователя.</returns>
    Task<UserData> GetCurrentUserAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);
}