namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с API друзей социальной сети Одноклассники (OK.ru).
/// </summary>
public interface IFriendsApiClient
{
    /// <summary>
    /// Получает список идентификаторов друзей текущего авторизованного пользователя.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов друзей.</returns>
    Task<ICollection<string>> GetUserFriendsAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список идентификаторов друзей указанного пользователя.
    /// </summary>
    /// <remarks>
    /// Для успешного выполнения запроса необходимо, чтобы запрашиваемый пользователь 
    /// разрешил доступ к своему списку друзей, а приложение имело соответствующие права.
    /// </remarks>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="friendId">Идентификатор пользователя, чьих друзей нужно получить.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов друзей указанного пользователя.</returns>
    Task<ICollection<string>> GetFriendsOfaFriendAsync(
        string accessToken,
        string sessionSecretKey,
        string friendId,
        CancellationToken cancellationToken = default);
}