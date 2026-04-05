using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.IntegrationTests;

/// <summary>
/// Конфигурационные константы и учётные данные для интеграции с API «Одноклассники» (OK.ru).
/// Предназначен для публикации авиационного контента (уведомления, метео-сводки, NOTAM) в корпоративные группы и альбомы.
/// </summary>
/// <remarks>
/// <para>В продакшен-среде все чувствительные данные должны передаваться через переменные окружения или защищённое хранилище конфигураций.</para>
/// <para>Текущие значения приведены исключительно для целей отладки и тестирования.</para>
/// </remarks>
public static class TestSettings
{
        /// <summary>
    /// Публичный ключ приложения (Application Key), выданный в кабинете разработчика OK.ru.
    /// Обязательное поле для идентификации запросов к API.
    /// </summary>
    /// <remarks>
    /// В продакшене должно загружаться из переменной окружения <c>OK_APP_KEY</c>.
    /// Хардкод значения в коде создаёт риск утечки учётных данных.
    /// </remarks>
    public const string ApplicationKey = ""; // Environment.GetEnvironmentVariable("OK_APP_KEY");

    /// <summary>
    /// Секретный ключ приложения (Application Secret) для подписи запросов к API OK.ru.
    /// Используется в сочетании с <see cref="ApplicationKey"/> для аутентификации сервер-сервер.
    /// </summary>
    /// <remarks>
    /// <b>Внимание:</b> данное поле содержит чувствительные данные. В продакшене должно загружаться из переменной окружения <c>OK_APP_SECRET</c>.
    /// Не коммитьте реальные значения в систему контроля версий.
    /// </remarks>
    private const string ApplicationSecret = ""; // Environment.GetEnvironmentVariable("OK_APP_SECRET");

    /// <summary>
    /// Access-токен пользователя для выполнения операций от его имени (публикация в альбомы, обсуждения и т.д.).
    /// Должен иметь необходимые права (scopes), согласованные с политикой безопасности предприятия.
    /// </summary>
    /// <remarks>
    /// <b>Внимание:</b> токен предоставляет доступ к аккаунту пользователя. В продакшене должен загружаться из переменной окружения <c>OK_USER_TOKEN</c>.
    /// Срок действия токена может быть ограничен — предусмотрена логика обновления при необходимости.
    /// </remarks>
    private const string UserAccessToken = ""; // Environment.GetEnvironmentVariable("OK_USER_TOKEN");

    /// <summary>
    /// Пара учётных данных для аутентификации запросов: токен пользователя и секрет приложения.
    /// Используется сервисным слоем для формирования подписанных вызовов к API.
    /// </summary>
    public static readonly AccessPair AccessPair = new() 
    { 
        AccessToken = UserAccessToken, 
        SessionSecretKey = ApplicationSecret 
    };

    /// <summary>
    /// Идентификатор корпоративной группы в OK.ru, в которую публикуются авиационные уведомления.
    /// Формат: строковое представление числового ID группы.
    /// </summary>
    public static readonly GroupId GroupId = new("");

    /// <summary>
    /// Идентификатор альбома группы для размещения медиафайлов общего назначения (схемы, отчёты).
    /// Используется при загрузке вложений через метод публикации.
    /// </summary>
    public const string GroupAlbumId = "";

    /// <summary>
    /// Идентификатор фотоальбома группы для публикации изображений (фото ВПП, метеорадары, инфографика).
    /// </summary>
    public const string GroupPhotoAlbumId = "";

    /// <summary>
    /// Идентификатор личного альбома пользователя-оператора для тестовой публикации перед отправкой в группу.
    /// </summary>
    public const string UserAlbumId = "";

    /// <summary>
    /// Идентификатор тестового фото в личном альбоме пользователя. Используется в интеграционных тестах.
    /// </summary>
    public const string UserAlbumPhotoId = "";

    /// <summary>
    /// Идентификатор альбома друга/партнёра для кросс-публикации согласованного контента (при наличии прав).
    /// </summary>
    public const string FriendAlbumId = "";

    /// <summary>
    /// Идентификатор друга/партнёра в системе OK.ru для операций, требующих указания целевого пользователя.
    /// </summary>
    public static readonly FriendId FriendId = new("");

    /// <summary>
    /// Идентификатор обсуждения (темы) в группе для публикации текстовых уведомлений и комментариев.
    /// Используется при отправке структурированных сообщений (NOTAM, метео-предупреждения).
    /// </summary>
    public const string DiscussionId = "";

    /// <summary>
    /// Проверяет наличие всех обязательных учётных данных для инициализации клиента OK.ru API.
    /// </summary>
    /// <returns>
    /// <c>true</c>, если <see cref="ApplicationKey"/>, <see cref="ApplicationSecret"/> и <see cref="UserAccessToken"/> не пустые;
    /// иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Рекомендуется вызывать перед первым обращением к сервису интеграции.
    /// При возврате <c>false</c> следует инициировать процедуру безопасного получения конфигурации из внешнего источника.
    /// </remarks>
    public static bool AreCredentialsAvailable =>
        !string.IsNullOrEmpty(ApplicationKey) &&
        !string.IsNullOrEmpty(ApplicationSecret) &&
        !string.IsNullOrEmpty(UserAccessToken);
}
