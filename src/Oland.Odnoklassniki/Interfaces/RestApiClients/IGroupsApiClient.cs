using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с группами в социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для получения информации о группах, проверки членства пользователей
/// и навигации по списку групп текущего пользователя. Поддерживает работу как с основным
/// аккаунтом (настроенным в IOkApiClientCore), так и с произвольными пользовательскими токенами
/// через параметр <c>context</c>.
/// </summary>
public interface IGroupsApiClient
{
    /// <summary>
    /// Получает информацию о группах по их идентификаторам.
    /// </summary>
    /// <param name="groupIds">Коллекция идентификаторов групп в формате OK.ru (строковые числовые значения).</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="fields">Добавление полей из <see cref="GroupBeanFields"/></param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Коллекция объектов <see cref="GroupInfoDto"/> с данными о запрошенных группах,
    /// или <c>null</c>, если ни одна группа не найдена или ответ сервера пуст.
    /// </returns>
    /// <remarks>
    /// Метод выполняет пакетный запрос к внешнему API OK.ru. Группы, недоступные для текущего
    /// пользователя (например, приватные или удалённые), исключаются из результата без ошибки.
    /// <list type="bullet">
    /// <item><description>Максимальное количество идентификаторов в одном запросе определяется лимитами платформы.</description></item>
    /// <item><description>Для получения дополнительных полей группы используйте параметры расширения, если поддерживаются.</description></item>
    /// <item><description>При ошибке аутентификации или превышении частоты запросов выбрасывается соответствующее исключение.</description></item>
    /// </list>
    /// </remarks>
    Task<ICollection<T>?> GetGroupsInfoAsync<T>(
        ICollection<string> groupIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where T : BaseOkDto;

    /// <summary>
    /// Получает информацию о статусе членства указанных пользователей в заданной группе.
    /// </summary>
    /// <param name="groupId">Идентификатор группы в формате OK.ru.</param>
    /// <param name="userIds">Коллекция идентификаторов пользователей для проверки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Коллекция объектов <see cref="GroupUserInfoDto"/> со статусом каждого пользователя,
    /// или <c>null</c>, если данные недоступны или запрос не вернул результатов.
    /// </returns>
    /// <remarks>
    /// Метод использует только основные учётные данные, настроенные в ядре клиента (не поддерживает
    /// передачу произвольного <c>context</c>). Требует прав на чтение состава группы.
    /// <list type="bullet">
    /// <item><description>Пользователи, не найденные или не имеющие отношения к группе, исключаются из ответа.</description></item>
    /// <item><description>Статус членства может включать значения: «участник», «модератор», «владелец», «не участник».</description></item>
    /// <item><description>Для массовых проверок рекомендуется группировать запросы для соблюдения rate-limit платформы.</description></item>
    /// </list>
    /// </remarks>
    Task<ICollection<GroupUserInfoDto>?> GetUserGroupsInfoByIdsAsync(
        GroupId groupId,
        ICollection<string> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для постраничного получения списка групп, в которых состоит текущий пользователь.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="anchorConfiguration">Настройки пагинации и якоря для навигации по коллекции.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Экземпляр <see cref="AnchorNavigator{UserGroupDto}"/> для итерации по результатам с поддержкой
    /// курсорной пагинации.
    /// </returns>
    /// <remarks>
    /// Метод возвращает только группы, доступные для чтения текущему пользователю. Порядок групп
    /// определяется сервером OK.ru и может не соответствовать хронологии вступления.
    /// <list type="bullet">
    /// <item><description>По умолчанию возвращаются базовые поля: идентификатор, название, тип группы, аватар.</description></item>
    /// <item><description>Для расширения набора полей используйте параметры конфигурации, если поддерживаются.</description></item>
    /// <item><description>Пагинация реализуется через механизм anchor/cursor, а не через offset/limit.</description></item>
    /// </list>
    /// </remarks>
    AnchorNavigator<UserGroupDto> GetUserGroupsAnchorNavigator(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken = default);
}