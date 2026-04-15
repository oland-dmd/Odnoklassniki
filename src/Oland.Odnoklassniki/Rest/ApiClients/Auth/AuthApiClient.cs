using Microsoft.Extensions.Logging;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Auth;

/// <summary>
/// Клиент для работы с методами авторизации и управления сессиями в API Одноклассников.
/// Обеспечивает продление срока действия сессий как для пользователей, так и для приложения.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр <see cref="AuthApiClient"/>.
/// </remarks>
/// <param name="clientCore">Ядро клиента OK.ru API для выполнения подписанных запросов.</param>
public class AuthApiClient(IOkApiClientCore clientCore, ILogger<AuthApiClient> logger) : IAuthApiClient
{
    private const string OkClassName = "auth";
    private const string TouchSessionMethodName = $"{OkClassName}.touchSession";

    /// <inheritdoc />
    public async Task<bool> TouchAccountSessionAsync(IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }
        
        return await clientCore.CallAsync<bool>(
            methodName: TouchSessionMethodName,
            context.AccessPair,
            cancellationToken: cancellationToken);
    }
}