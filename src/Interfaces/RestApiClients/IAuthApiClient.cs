namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с методами авторизации и управления сессиями в API Одноклассников.
/// </summary>
public interface IAuthApiClient
{
    /// <summary>
    /// Продлевает срок действия пользовательской сессии.
    /// </summary>
    /// <remarks>
    /// Должен вызываться не реже чем раз в 30 минут при бездействии.
    /// Для OAuth-сессий с правом LONG_ACCESS_TOKEN устанавливает новый срок действия — 30 дней.
    /// Ограничение: не более 10 вызовов в месяц на пользователя.
    /// </remarks>
    /// <param name="accessToken">Токен доступа пользователя (OAuth-токен).</param>
    /// <param name="sessionSecretKey">Секретный ключ приложения для подписи запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сессия успешно продлена; иначе — false.</returns>
    Task<bool> TouchAccountSessionAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Продлевает срок действия основной сессии приложения.
    /// </summary>
    /// <remarks>
    /// Использует учетные данные, настроенные внутри IOkApiClientCore.
    /// Предназначен для поддержания активности основного аккаунта приложения (Prod/Dev).
    /// </remarks>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сессия приложения успешно продлена; иначе — false.</returns>
    Task<bool> TouchMainSessionAsync(CancellationToken cancellationToken = default);
}