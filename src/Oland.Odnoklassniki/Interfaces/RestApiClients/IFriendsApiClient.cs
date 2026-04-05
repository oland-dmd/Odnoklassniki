using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с API друзей социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для получения и анализа связей между пользователями.
/// Данные извлекаются из профиля текущего авторизованного пользователя через внешнее API OK.ru.
/// </summary>
public interface IFriendsApiClient
{
    /// <summary>
    /// Получает список идентификаторов друзей текущего авторизованного пользователя.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Коллекция идентификаторов друзей в формате OK.ru (строковые числовые значения).
    /// Возвращает пустую коллекцию, если у пользователя нет друзей или доступ к списку ограничен.
    /// </returns>
    /// <remarks>
    /// Метод требует наличия валидной сессии и прав доступа <c>VALUABLE_ACCESS</c> или эквивалентных.
    /// Возвращаемые идентификаторы соответствуют внутреннему формату OK.ru и могут использоваться
    /// в других методах API для получения детальной информации о пользователях.
    /// <list type="bullet">
    /// <item><description>Список не включает заблокированных и скрытых пользователей.</description></item>
    /// <item><description>Порядок элементов не гарантируется и определяется сервером OK.ru.</description></item>
    /// <item><description>При ошибке аутентификации или недостаточных правах выбрасывается соответствующее исключение.</description></item>
    /// </list>
    /// </remarks>
    Task<ICollection<string>> GetUserFriendsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);
}