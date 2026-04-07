namespace Oland.Odnoklassniki.Rest.ApiClientCore;

/// <summary>
/// Модель конфигурации для инициализации клиента API Одноклассников.
/// Содержит учетные данные, необходимые для авторизации и подписи запросов.
/// </summary>

public class ApplicationOptions
{
    /// <summary>
    /// Уникальный идентификатор приложения (Application Key). Используется для идентификации клиента в API. Обязательное поле.
    /// </summary>
    public required string ApplicationKey { get; init; }

    /// <summary>
    /// Токен доступа для авторизации запросов. Определяет права доступа и владельца сессии. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Требует защищенного хранения. Не рекомендуется передавать в логах или клиентском коде.
    /// </remarks>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Секретный ключ сессии для подписи запросов и валидации данных. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Критически важные данные. Компрометация ключа позволяет подделывать запросы от имени приложения.
    /// </remarks>
    public required string SessionSecretKey { get; init; }

    /// <summary>
    /// Идентификатор пользовательского агента (User-Agent) для HTTP-запросов к API.
    /// Необязательное поле. Если не задан, используется значение по умолчанию, установленное в HTTP-клиенте.
    /// </summary>
    /// <remarks>
    /// Рекомендуется указывать в формате: <c>«AppName/1.0 (contact@example.com)»</c> для упрощения идентификации трафика
    /// на стороне сервиса Одноклассников и при отладке запросов.
    /// </remarks>
    public string? UserAgent { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public required string GroupId { get; init; }
}