using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;

namespace Odnoklassniki.Rest.ApiClients.Auth;

/// <summary>
/// Клиент для работы с методами авторизации и управления сессиями в API Одноклассников.
/// Обеспечивает продление срока действия сессий как для пользователей, так и для приложения.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр <see cref="AuthApiClient"/>.
/// </remarks>
/// <param name="clientCore">Ядро клиента OK.ru API для выполнения подписанных запросов.</param>
public class AuthApiClient(IOkApiClientCore clientCore) : IAuthApiClient
{
    private const string OkClassName = "auth";
    private const string TouchSessionMethodName = $"{OkClassName}.touchSession";

    /// <summary>
    /// Продлевает срок действия **пользовательской сессии**.
    /// Должен вызываться не реже чем раз в 30 минут при бездействии.
    /// Для OAuth-сессий с правом <c>LONG_ACCESS_TOKEN</c> устанавливает новый срок действия — 30 дней.
    /// Ограничение: не более 10 вызовов в месяц на пользователя.
    /// </summary>
    /// <param name="accessToken">
    /// Токен доступа пользователя (OAuth-токен), полученный при авторизации.
    /// </param>
    /// <param name="sessionSecretKey">
    /// Секретный ключ приложения (application secret), используемый для генерации подписи запроса.
    /// </param>
    /// <param name="cancellationToken">
    /// Токен отмены операции.
    /// </param>
    /// <returns>
    /// <see langword="true"/>, если сессия успешно продлена; иначе — <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Возникает, если <paramref name="accessToken"/> или <paramref name="sessionSecretKey"/> — null или пустые.
    /// </exception>
    /// <exception cref="Refit.ApiException">
    /// Возникает при ошибках сети, недействительной подписи или превышении лимита вызовов.
    /// </exception>
    public async Task<bool> TouchAccountSessionAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default)
    {
        return await clientCore.CallAsync<bool>(
            methodName: TouchSessionMethodName,
            accessToken: accessToken,
            secret: sessionSecretKey,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Продлевает срок действия **основной сессии приложения** (например, application access token),
    /// настроенной внутри реализации <see cref="IOkApiClientCore"/>.
    /// Используется для поддержания активности основного аккаунта приложения Prod Akk или Dev Akk.
    /// </summary>
    /// <param name="cancellationToken">
    /// Токен отмены операции.
    /// </param>
    /// <returns>
    /// <see langword="true"/>, если сессия приложения успешно продлена; иначе — <see langword="false"/>.
    /// </returns>
    /// <exception cref="Refit.ApiException">
    /// Возникает, если клиент не настроен с учётными данными приложения или при ошибках API.
    /// </exception>
    public async Task<bool> TouchMainSessionAsync(CancellationToken cancellationToken = default)
    {
        return await clientCore.CallAsync<bool>(TouchSessionMethodName, cancellationToken: cancellationToken);
    }
}