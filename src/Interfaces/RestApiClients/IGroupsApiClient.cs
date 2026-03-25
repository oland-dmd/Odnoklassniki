using Odnoklassniki.Enums;
using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Groups.Dtos;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с группами в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public interface IGroupsApiClient
{
    #region Get Group Info

    /// <summary>
    /// Получает информацию о группах по их идентификаторам (основной аккаунт).
    /// </summary>
    /// <param name="groupIds">Коллекция идентификаторов групп.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция данных о группах или null, если ответ пуст.</returns>
    Task<ICollection<GroupInfoDto>?> GetGroupsInfoAsync(
        ICollection<string> groupIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает информацию о группах по их идентификаторам с указанными токенами.
    /// </summary>
    /// <param name="groupIds">Коллекция идентификаторов групп.</param>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция данных о группах или null, если ответ пуст.</returns>
    Task<ICollection<GroupInfoDto>?> GetGroupsInfoAsync(
        ICollection<string> groupIds,
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get User Groups Info

    /// <summary>
    /// Получает информацию о членстве пользователей в группе (основной аккаунт).
    /// </summary>
    /// <remarks>
    /// Этот метод использует только основные учетные данные, настроенные в ядре клиента.
    /// </remarks>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="userIds">Коллекция идентификаторов пользователей.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция данных о статусе пользователей или null.</returns>
    Task<ICollection<GroupUserInfoDto>?> GetUserGroupsInfoByIdsAsync(
        string groupId,
        ICollection<string> userIds,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get User Groups (V2 with Pagination)

    /// <summary>
    /// Получает список групп пользователя с поддержкой пагинации (основной аккаунт).
    /// </summary>
    /// <param name="anchor">Метка пагинации.</param>
    /// <param name="direction">Направление пагинации.</param>
    /// <param name="count">Количество записей на странице.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ с данными о группах и пагинации.</returns>
    AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(string anchor,
        PagingDirection direction,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список групп пользователя с поддержкой пагинации с указанными токенами.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии.</param>
    /// <param name="direction">Направление пагинации.</param>
    /// <param name="count">Количество записей на странице.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ответ с данными о группах и пагинации.</returns>
    AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(string accessToken,
        string sessionSecretKey,
        PagingDirection direction,
        int count = 100,
        CancellationToken cancellationToken = default);

    #endregion
}