using Odnoklassniki.Rest.RequestContexts;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с методами авторизации и управления сессиями в API Одноклассников.
/// Предоставляет функции контроля времени жизни пользовательских сессий и управления токенами доступа.
/// </summary>
public interface IAuthApiClient
{
    /// <summary>
    /// Продлевает срок действия пользовательской сессии.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и идентификатор сессии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// <c>true</c>, если сессия успешно продлена; <c>false</c>, если продление невозможно 
    /// (например, истёк максимальный срок жизни сессии или превышен лимит вызовов).
    /// </returns>
    /// <remarks>
    /// Метод должен вызываться не реже чем раз в 30 минут при бездействии пользователя для предотвращения 
    /// автоматического завершения сессии. Для OAuth-сессий с правом <c>LONG_ACCESS_TOKEN</c> устанавливает 
    /// новый срок действия — 30 дней с момента последнего успешного вызова.
    /// <list type="bullet">
    /// <item><description>Ограничение частоты: не более 10 вызовов в месяц на одного пользователя.</description></item>
    /// <item><description>При превышении лимита метод возвращает <c>false</c> без продления сессии.</description></item>
    /// <item><description>Не используется для первоначальной аутентификации — только для поддержания активной сессии.</description></item>
    /// </list>
    /// </remarks>
    Task<bool> TouchAccountSessionAsync(IRequestContext context,
        CancellationToken cancellationToken = default);
}