using Oland.Odnoklassniki.Rest.ApiClients.Friends.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с API друзей социальной сети Одноклассники (OK.ru).
/// Все запросы требуют scope <c>VALUABLE_ACCESS</c>.
/// </summary>
public interface IFriendsApiClient
{
    /// <summary>
    /// Получает список идентификаторов всех друзей текущего пользователя (<c>friends.get</c>).
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов друзей. Возвращает пустую коллекцию, если друзей нет.</returns>
    Task<ICollection<string>> GetUserFriendsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает идентификаторы друзей, установивших приложение (<c>friends.getAppUsers</c>).
    /// Ответ: <c>{"uids": [...]}</c>.
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов или <c>null</c>.</returns>
    Task<ICollection<string>?> GetAppUsersAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает идентификаторы друзей, находящихся в сети прямо сейчас (<c>friends.getOnline</c>).
    /// API возвращает прямой массив идентификаторов.
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов онлайн-друзей или <c>null</c>.</returns>
    Task<ICollection<string>?> GetOnlineAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список общих друзей с указанным пользователем (<c>friends.getMutualFriends</c>).
    /// Параметр запроса: <c>target_id</c>. API возвращает прямой массив идентификаторов.
    /// </summary>
    /// <param name="targetUid">Идентификатор целевого пользователя (<c>target_id</c>).</param>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов общих друзей или <c>null</c>.</returns>
    Task<ICollection<string>?> GetMutualFriendsAsync(
        string targetUid,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает идентификаторы друзей, использующих указанные платформы (<c>friends.getByDevices</c>).
    /// Параметр запроса: <c>devices</c>. API возвращает прямой массив идентификаторов.
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод помечен как устаревший (deprecated) на стороне OK.ru.
    /// </remarks>
    /// <param name="devices">Список платформ: например, <c>"OK"</c>, <c>"MOBILE"</c>, <c>"WEB"</c>.</param>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов или <c>null</c>.</returns>
    Task<ICollection<string>?> GetByDevicesAsync(
        IEnumerable<string> devices,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список друзей с предстоящими днями рождения (<c>friends.getBirthdays</c>).
    /// API возвращает прямой массив <c>[{"uid", "date"}]</c>.
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="future">
    /// <c>false</c> (по умолчанию) — дни рождения в ближайшие 3 дня;
    /// <c>true</c> — дни рождения в ближайшие 30 дней.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция <see cref="UserBirthdayDto"/> или <c>null</c>.</returns>
    Task<ICollection<UserBirthdayDto>?> GetBirthdaysAsync(
        IRequestContext context,
        bool future = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает рекомендуемых друзей (кого вы, возможно, знаете) (<c>friends.getSuggestions</c>).
    /// Возвращает только первую страницу (пагинация через anchor не реализована).
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="FriendRequestContext"/>, <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция идентификаторов предлагаемых пользователей или <c>null</c>.</returns>
    Task<ICollection<string>?> GetSuggestionsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);
}
