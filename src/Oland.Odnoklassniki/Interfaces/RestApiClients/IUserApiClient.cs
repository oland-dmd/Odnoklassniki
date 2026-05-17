using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с данными пользователей в социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для получения профиля, дополнительной информации,
/// управления статусом и проверки разрешений приложения.
/// </summary>
public interface IUserApiClient
{
    /// <summary>
    /// Получает уникальный идентификатор (UID) текущего авторизованного пользователя (<c>users.getLoggedInUser</c>).
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Строковый UID или <c>null</c>.</returns>
    Task<string?> GetLoggedInUserAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает расширенные данные профиля текущего авторизованного пользователя (<c>users.getCurrentUser</c>).
    /// </summary>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="UserData"/> с основной информацией.</returns>
    Task<UserData> GetCurrentUserAsync(IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает информацию о нескольких пользователях по их идентификаторам (<c>users.getInfo</c>).
    /// Максимум 100 идентификаторов за запрос. Требует scope <c>VALUABLE_ACCESS</c>.
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, наследующий <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="uids">Коллекция идентификаторов пользователей (до 100 штук).</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="fields">Список запрашиваемых полей из <see cref="Oland.Odnoklassniki.Rest.BeanFields.UserBeanFields"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция DTO пользователей или <c>null</c>.</returns>
    Task<ICollection<TDto>?> GetInfoAsync<TDto>(
        ICollection<string> uids,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Получает информацию об одном пользователе по его идентификатору (<c>users.getInfoBy</c>).
    /// Ответ API обёрнут в поле <c>"user"</c>. Требует scope <c>VALUABLE_ACCESS</c>.
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, наследующий <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="uid">Идентификатор пользователя.</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="fields">Список запрашиваемых полей из <see cref="Oland.Odnoklassniki.Rest.BeanFields.UserBeanFields"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>DTO пользователя или <c>null</c>.</returns>
    Task<TDto?> GetInfoByAsync<TDto>(
        string uid,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Получает дополнительную статистическую информацию о пользователях (<c>users.getAdditionalInfo</c>):
    /// платёжная активность, игровая активность, баланс OK-валюты. Максимум 100 идентификаторов.
    /// Данные обновляются с задержкой до 24 часов. Требует scope <c>VALUABLE_ACCESS</c>.
    /// </summary>
    /// <param name="uids">Коллекция идентификаторов пользователей (до 100 штук).</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция <see cref="UserAdditionalInfoData"/> или <c>null</c>.</returns>
    Task<ICollection<UserAdditionalInfoData>?> GetAdditionalInfoAsync(
        ICollection<string> uids,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Устанавливает текстовый статус текущего пользователя (<c>users.setStatus</c>).
    /// Требует scope <c>SET_STATUS</c>.
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод помечен как устаревший (deprecated) на стороне OK.ru.
    /// Для очистки статуса передайте <c>null</c> (параметр не будет включён в запрос).
    /// </remarks>
    /// <param name="status">Текст статуса. <c>null</c> — очистить статус.</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если статус успешно обновлён.</returns>
    Task<bool> SetStatusAsync(
        string? status,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, предоставил ли пользователь указанное разрешение приложению (<c>users.hasAppPermission</c>).
    /// Параметр разрешения передаётся как <c>ext_perm</c>.
    /// </summary>
    /// <param name="permission">Название разрешения из <c>ApplicationPermission</c> (например, <c>VALUABLE_ACCESS</c>).</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если разрешение предоставлено.</returns>
    Task<bool> HasAppPermissionAsync(
        string permission,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, установлено ли приложение у указанного пользователя (<c>users.isAppUser</c>).
    /// </summary>
    /// <param name="uid">Идентификатор проверяемого пользователя.</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если пользователь установил приложение.</returns>
    Task<bool> IsAppUserAsync(
        string uid,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает количество оставшихся вызовов API для указанных методов (<c>users.getCallsLeft</c>).
    /// Для проверки <c>showNotification</c> используйте имя <c>"notifications.sendFromUser"</c>.
    /// </summary>
    /// <param name="methods">Список имён методов для проверки лимитов.</param>
    /// <param name="context">Допустимые типы: <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция <see cref="MethodCallsLeftData"/> по каждому методу или <c>null</c>.</returns>
    Task<ICollection<MethodCallsLeftData>?> GetCallsLeftAsync(
        ICollection<string> methods,
        IRequestContext context,
        CancellationToken cancellationToken = default);
}
