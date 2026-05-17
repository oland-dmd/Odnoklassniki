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

    /// <summary>
    /// Возвращает навигатор для постраничного перебора участников группы.
    /// Соответствует методу <c>group.getMembers</c>.
    /// </summary>
    /// <remarks>
    /// ⚠️ Параметр идентификатора группы передаётся как <c>uid</c> (нестандартно для этого метода API).
    /// Поддерживает фильтрацию по статусу участника через <paramref name="statusFilter"/>.
    /// </remarks>
    /// <param name="context">Контекст запроса: <see cref="GroupRequestContext"/> или <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="cfg">Настройки курсорной пагинации.</param>
    /// <param name="statusFilter">Необязательный фильтр по статусу участника.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор <see cref="AnchorNavigator{GroupMemberDto}"/> по списку участников.</returns>
    AnchorNavigator<GroupMemberDto> GetMembersNavigator(
        IRequestContext context,
        AnchorConfiguration cfg,
        GroupMemberStatusFilter? statusFilter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает счётчики группы (участники, фото, видео и т.д.).
    /// Соответствует методу <c>group.getCounters</c>.
    /// </summary>
    /// <param name="context">Контекст запроса: <see cref="GroupRequestContext"/> или <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="counterTypes">
    /// Список типов запрашиваемых счётчиков (например, <c>MEMBERS</c>, <c>PHOTOS</c>).
    /// При <see langword="null"/> сервер возвращает счётчики по умолчанию.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="GroupCountersDto"/> со значениями счётчиков, или <see langword="null"/>.</returns>
    Task<GroupCountersDto?> GetCountersAsync(
        IRequestContext context,
        IEnumerable<string>? counterTypes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает сводную статистику группы за период.
    /// Соответствует методу <c>group.getStatOverview</c>.
    /// </summary>
    /// <remarks>Требует прав администратора группы и scope <c>GROUP_CONTENT</c>.</remarks>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="period">Тип периода агрегации (WEEK или MONTH). При <see langword="null"/> сервер использует значение по умолчанию.</param>
    /// <param name="startTime">Начало диапазона (Unix мс). При <see langword="null"/> сервер определяет самостоятельно.</param>
    /// <param name="fields">Список запрашиваемых полей статистики. Рекомендуется передавать явно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="GroupStatOverviewDto"/>, или <see langword="null"/>.</returns>
    Task<GroupStatOverviewDto?> GetStatOverviewAsync(
        IRequestContext context,
        GroupStatPeriod? period = null,
        long? startTime = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает демографическую статистику аудитории группы.
    /// Соответствует методу <c>group.getStatPeople</c>.
    /// </summary>
    /// <remarks>Требует прав администратора группы и scope <c>GROUP_CONTENT</c>.</remarks>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="demoType">Тип демографической разбивки (например, <c>gender</c>, <c>age</c>). При <see langword="null"/> возвращаются все типы.</param>
    /// <param name="fields">Список запрашиваемых полей. Рекомендуется передавать явно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="GroupStatPeopleDto"/>, или <see langword="null"/>.</returns>
    Task<GroupStatPeopleDto?> GetStatPeopleAsync(
        IRequestContext context,
        string? demoType = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает статистику одного поста (топика) группы.
    /// Соответствует методу <c>group.getStatTopic</c>.
    /// </summary>
    /// <remarks>Требует прав администратора группы и scope <c>GROUP_CONTENT</c>.</remarks>
    /// <param name="topicId">Идентификатор топика.</param>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="fields">Список запрашиваемых полей статистики.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="GroupStatTopicDto"/>, или <see langword="null"/>.</returns>
    Task<GroupStatTopicDto?> GetStatTopicAsync(
        string topicId,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для постраничного перебора статистики постов группы за диапазон дат.
    /// Соответствует методу <c>group.getStatTopics</c>.
    /// </summary>
    /// <remarks>Требует прав администратора группы и scope <c>GROUP_CONTENT</c>.</remarks>
    /// <param name="startTime">Начало диапазона (Unix мс).</param>
    /// <param name="endTime">Конец диапазона (Unix мс).</param>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="cfg">Настройки курсорной пагинации.</param>
    /// <param name="fields">Список запрашиваемых полей статистики.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор <see cref="AnchorNavigator{GroupStatTopicDto}"/> по статистике постов.</returns>
    AnchorNavigator<GroupStatTopicDto> GetStatTopicsNavigator(
        long startTime,
        long endTime,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает временны́е ряды (тренды) ключевых показателей группы за диапазон дат.
    /// Соответствует методу <c>group.getStatTrends</c>.
    /// </summary>
    /// <remarks>Требует прав администратора группы и scope <c>GROUP_CONTENT</c>.</remarks>
    /// <param name="startTime">Начало диапазона (Unix мс).</param>
    /// <param name="endTime">Конец диапазона (Unix мс).</param>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="fields">Список запрашиваемых показателей (например, <c>views</c>, <c>likes</c>).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Объект <see cref="GroupStatTrendsDto"/> с временны́ми рядами, или <see langword="null"/>.</returns>
    Task<GroupStatTrendsDto?> GetStatTrendsAsync(
        long startTime,
        long endTime,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Закрепляет запись в ленте группы с помощью одноразового токена.
    /// Соответствует методу <c>group.pinGroupFeed</c>.
    /// </summary>
    /// <remarks>
    /// Параметр <paramref name="pinId"/> — одноразовый токен из поля <c>pin_id</c>,
    /// который возвращается при запросе топика через <c>mediatopic.getByIds</c>.
    /// Требует scope <c>GROUP_CONTENT</c>.
    /// </remarks>
    /// <param name="pinId">Одноразовый токен закрепления.</param>
    /// <param name="context">Контекст запроса: <see cref="GroupRequestContext"/> или <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/>, если запись успешно закреплена.</returns>
    Task<bool> PinGroupFeedAsync(
        string pinId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Устанавливает главное фото группы.
    /// Соответствует методу <c>group.setMainPhoto</c>.
    /// </summary>
    /// <remarks>Требует scope <c>GROUP_CONTENT</c> и <c>PHOTO_CONTENT</c>.</remarks>
    /// <param name="photoId">Идентификатор фотографии, которую нужно сделать главной.</param>
    /// <param name="context">Контекст запроса: только <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/>, если фото успешно установлено.</returns>
    Task<bool> SetMainPhotoAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, разрешены ли сообщения от группы текущему пользователю.
    /// Соответствует методу <c>group.isMessagesAllowed</c>.
    /// </summary>
    /// <param name="context">Контекст запроса: <see cref="GroupRequestContext"/> или <see cref="MainGroupRequestContext"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/>, если пользователь принимает сообщения от группы.</returns>
    Task<bool> IsMessagesAllowedAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);
}