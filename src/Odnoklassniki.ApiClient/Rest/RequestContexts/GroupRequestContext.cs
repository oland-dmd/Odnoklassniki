using Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Контекст запроса для получения информации о группе в социальной сети Одноклассники (OK.ru).
/// Реализует <see cref="IRequestContext"/> и обеспечивает передачу идентификатора группы в параметрах запроса,
/// а также поддержку авторизации как через явные учётные данные, так и через основной аккаунт клиента.
/// </summary>
/// <remarks>
/// Используется в сценариях, когда необходимо выполнить запрос к API для получения данных о конкретной группе:
/// информация о составе, настройках приватности, публикациях и других метаданных. Идентификатор группы
/// автоматически добавляется в параметры запроса через метод <see cref="Apply"/>.
/// <list type="bullet">
/// <item><description>Поддерживает два режима инициализации: с явными токенами (<see cref="AccessPair"/>) и с использованием основного аккаунта.</description></item>
/// <item><description>При отсутствии <see cref="AccessPair"/> запрос выполняется в контексте приложения или основной сессии, настроенной в <see cref="IOkApiClientCore"/>.</description></item>
/// <item><description>Не передавайте экземпляры этого класса в логах — они могут содержать конфиденциальные учётные данные.</description></item>
/// </list>
/// </remarks>
public record GroupRequestContext : IRequestContext
{
    /// <summary>
    /// Идентификатор группы, для которой выполняется запрос.
    /// </summary>
    /// <remarks>
    /// Значение передаётся в параметрах запроса через метод <see cref="Apply"/> с использованием
    /// расширения <c>InsertGroupId</c>. Формат идентификатора должен соответствовать спецификации OK.ru
    /// (обычно строковое числовое значение). Валидация формата выполняется при создании <see cref="GroupId"/>.
    /// Инициализируется значением по умолчанию в объявлении свойства, но обязательно перезаписывается в конструкторе.
    /// </remarks>
    public GroupId GroupId { get; }

    /// <summary>
    /// Пара учётных данных для авторизации запроса: токен доступа и секрет сессии.
    /// </summary>
    /// <remarks>
    /// Необязательное поле. Если не задано, запрос выполняется с использованием основных учётных данных,
    /// настроенных в ядре клиента <see cref="IOkApiClientCore"/>. При наличии значения используется
    /// для расчёта криптографической подписи (<c>sig</c>) запроса к внешнему API.
    /// <list type="bullet">
    /// <item><description>Содержит конфиденциальные данные — не логируйте и не передавайте вовне.</description></item>
    /// <item><description>Инициализируется только в конструкторе с параметром <see cref="AccessPair"/>.</description></item>
    /// </list>
    /// </remarks>
    public AccessPair AccessPair { get; }

    /// <summary>
    /// Инициализирует контекст для работы с группой с использованием основного аккаунта.
    /// </summary>
    /// <param name="groupId">
    /// Идентификатор группы, данные которой необходимо получить или изменить.
    /// Обязательный параметр, проходит валидацию при создании <see cref="GroupId"/>.
    /// </param>
    /// <remarks>
    /// Данный конструктор используется, когда запрос выполняется в контексте основной сессии,
    /// настроенной в <see cref="IOkApiClientCore"/>. Учётные данные для подписи запроса
    /// будут взяты из глобальных настроек клиента.
    /// </remarks>
    public GroupRequestContext(GroupId groupId)
    {
        GroupId = groupId;
    }

    /// <summary>
    /// Инициализирует контекст для работы с группой с явным указанием учётных данных.
    /// </summary>
    /// <param name="accessPair">
    /// Пара <see cref="AccessPair"/>, содержащая токен доступа и секрет сессии для авторизации запроса.
    /// Используется для выполнения запроса от имени конкретного пользователя или приложения.
    /// </param>
    /// <param name="groupId">
    /// Идентификатор группы, данные которой необходимо получить или изменить.
    /// Обязательный параметр, проходит валидацию при создании <see cref="GroupId"/>.
    /// </param>
    /// <remarks>
    /// Данный конструктор используется в многопользовательских сценариях или при работе с несколькими группами,
    /// когда запросы выполняются с разными учётными данными без пересоздания основного клиента.
    /// </remarks>
    public GroupRequestContext(AccessPair accessPair, GroupId groupId)
    {
        GroupId = groupId;
        AccessPair = accessPair;
    }

    /// <summary>
    /// Деконструирует контекст на отдельные компоненты учётных данных.
    /// </summary>
    /// <param name="accessToken">
    /// Возвращает токен доступа из <see cref="AccessPair"/>, или пустую строку,
    /// если учётные данные не были явно заданы.
    /// </param>
    /// <param name="sessionSecretKey">
    /// Возвращает секрет сессии из <see cref="AccessPair"/>, или пустую строку,
    /// если учётные данные не были явно заданы.
    /// </param>
    /// <remarks>
    /// Позволяет использовать паттерн деконструкции для извлечения учётных данных:
    /// <code>
    /// var (token, secret) = requestContext;
    /// </code>
    /// Если контекст инициализирован без <see cref="AccessPair"/>, возвращаются пустые строки.
    /// Не сохраняйте извлечённые значения вне контекста выполнения запроса.
    /// </remarks>
    public void Deconstruct(out string accessToken, out string sessionSecretKey)
    {
        accessToken = AccessPair.AccessToken;
        sessionSecretKey = AccessPair.SessionSecretKey;
    }

    /// <summary>
    /// Добавляет идентификатор группы в параметры запроса.
    /// </summary>
    /// <param name="parameters">Исходные параметры запроса типа <see cref="RestParameters"/>.</param>
    /// <returns>
    /// Новый экземпляр <see cref="RestParameters"/> с добавленным параметром <c>group_id</c>,
    /// значение которого равно <see cref="GroupId.Value"/>.
    /// </returns>
    /// <remarks>
    /// Метод реализует контракт <see cref="IRequestContext.Apply"/>.
    /// Использует расширение <c>InsertGroupId</c> для безопасного добавления идентификатора
    /// в коллекцию параметров с учётом кодирования и валидации.
    /// <list type="bullet">
    /// <item><description>Исходный экземпляр <paramref name="parameters"/> не модифицируется — возвращается новый объект.</description></item>
    /// <item><description>Если параметр <c>group_id</c> уже присутствует, он перезаписывается значением из контекста.</description></item>
    /// <item><description>Подпись запроса (<c>sig</c>) рассчитывается после применения всех параметров на уровне ядра клиента.</description></item>
    /// </list>
    /// </remarks>
    public RestParameters Apply(RestParameters parameters)
    {
        return parameters.InsertGroupId(GroupId.Value);
    }
}