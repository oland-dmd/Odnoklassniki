using Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с данными пользователей в социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для получения идентификатора и профиля текущего авторизованного пользователя.
/// Все запросы выполняются через внешнее API OK.ru с использованием контекста аутентификации
/// <see cref="IRequestContext"/>.
/// </summary>
public interface IUserApiClient
{
    /// <summary>
    /// Получает уникальный идентификатор (UID) текущего авторизованного пользователя.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Строковый идентификатор пользователя в формате OK.ru, или <c>null</c>,
    /// если аутентификация не пройдена, сессия истекла или произошла ошибка при обработке запроса.
    /// </returns>
    /// <remarks>
    /// Метод выполняет лёгкий запрос к API для извлечения только идентификатора пользователя,
    /// без загрузки дополнительных данных профиля. Подходит для быстрой проверки состояния сессии.
    /// <list type="bullet">
    /// <item><description>Требует наличия валидного токена доступа в контексте запроса.</description></item>
    /// <item><description>Не выбрасывает исключение при ошибке аутентификации — возвращает <c>null</c>.</description></item>
    /// <item><description>Возвращаемый UID может использоваться в других методах API для идентификации пользователя.</description></item>
    /// </list>
    /// </remarks>
    Task<string?> GetLoggedInUserAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает расширенные данные профиля текущего авторизованного пользователя.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Объект <see cref="UserData"/> с полной информацией о пользователе:
    /// имя, фамилия, пол, дата рождения, город, ссылка на аватар и другие поля профиля.
    /// </returns>
    /// <remarks>
    /// Метод запрашивает данные из внешнего API OK.ru. Набор возвращаемых полей определяется
    /// правами доступа токена и настройками приватности пользователя.
    /// <list type="bullet">
    /// <item><description>Требуются права <c>VALUABLE_ACCESS</c> или эквивалентные для получения расширенных данных.</description></item>
    /// <item><description>При отсутствии прав или истечении сессии выбрасывает исключение аутентификации.</description></item>
    /// <item><description>Некоторые поля могут отсутствовать в ответе, если пользователь скрыл их в настройках приватности.</description></item>
    /// <item><description>Для кэширования данных профиля рекомендуется реализовывать слой кэша на стороне клиента.</description></item>
    /// </list>
    /// </remarks>
    Task<UserData> GetCurrentUserAsync(IRequestContext context,
        CancellationToken cancellationToken = default);
}