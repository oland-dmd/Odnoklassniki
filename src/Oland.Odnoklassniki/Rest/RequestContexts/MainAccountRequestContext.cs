using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Контекст запроса для работы с основным аккаунтом, настроенным в ядре клиента <see cref="IOkApiClientCore"/>.
/// Реализует <see cref="IRequestContext"/> и используется для выполнения запросов с использованием
/// глобальных учётных данных приложения или основной пользовательской сессии.
/// </summary>
/// <remarks>
/// Данный контекст применяется в сценариях, когда не требуется явная передача токенов для каждого запроса —
/// учётные данные берутся из конфигурации клиента при его инициализации.
/// <list type="bullet">
/// <item><description>Свойство <see cref="AccessPair"/> содержит пустые значения — фактические токены подставляются на уровне ядра клиента.</description></item>
/// <item><description>Метод <see cref="Apply"/> не модифицирует параметры запроса — используется стандартный набор параметров.</description></item>
/// <item><description>Подходит для однопользовательских приложений или сервисов с единой учётной записью.</description></item>
/// <item><description>Не используйте в многопользовательских сценариях — для них предназначен <see cref="ExplicitTokenRequestContext"/>.</description></item>
/// </list>
/// </remarks>
public record MainAccountRequestContext : IRequestContext
{
    /// <summary>
    /// Контекст запроса для работы с основным аккаунтом, настроенным в ядре клиента <see cref="IOkApiClientCore"/>.
    /// Реализует <see cref="IRequestContext"/> и используется для выполнения запросов с использованием
    /// глобальных учётных данных приложения или основной пользовательской сессии.
    /// </summary>
    /// <remarks>
    /// Данный контекст применяется в сценариях, когда не требуется явная передача токенов для каждого запроса —
    /// учётные данные берутся из конфигурации клиента при его инициализации.
    /// <list type="bullet">
    /// <item><description>Свойство <see cref="AccessPair"/> содержит пустые значения — фактические токены подставляются на уровне ядра клиента.</description></item>
    /// <item><description>Метод <see cref="Apply"/> не модифицирует параметры запроса — используется стандартный набор параметров.</description></item>
    /// <item><description>Подходит для однопользовательских приложений или сервисов с единой учётной записью.</description></item>
    /// <item><description>Не используйте в многопользовательских сценариях — для них предназначен <see cref="ExplicitTokenRequestContext"/>.</description></item>
    /// </list>
    /// </remarks>
    public MainAccountRequestContext(IOptions<ApplicationOptions> Options)
    {
        this.Options = Options;
        AccessPair = new AccessPair
        {
            AccessToken = Options.Value.AccessToken,
            SessionSecretKey = Options.Value.SessionSecretKey,
        };
    }

    /// <summary>
    /// Пара учётных данных для авторизации запроса. В данной реализации содержит пустые значения.
    /// </summary>
    /// <remarks>
    /// Значения по умолчанию (<c>AccessToken = ""</c>, <c>SessionSecretKey = ""</c>) означают,
    /// что фактические учётные данные для расчёта подписи (<c>sig</c>) берутся из конфигурации
    /// <see cref="IOkApiClientCore"/> при отправке запроса.
    /// <list type="bullet">
    /// <item><description>Для основного аккаунта используются <c>application_secret</c> или глобальный токен,
    /// настроенные при инициализации клиента.</description></item>
    /// <item><description>Не передавайте это свойство во внешние системы — оно служит маркером режима работы.</description></item>
    /// <item><description>При необходимости явной передачи токенов используйте <see cref="ExplicitTokenRequestContext"/>.</description></item>
    /// </list>
    /// </remarks>
    public AccessPair AccessPair { get; }

    public IOptions<ApplicationOptions> Options { get; init; }

    /// <summary>
    /// Применяет контекст к параметрам запроса. В данной реализации не модифицирует входные данные.
    /// </summary>
    /// <param name="parameters">Исходные параметры запроса типа <see cref="RestParameters"/>.</param>
    /// <returns>
    /// Тот же экземпляр <see cref="RestParameters"/>, без изменений.
    /// Параметры авторизации и подписи добавляются на уровне ядра клиента.
    /// </returns>
    /// <remarks>
    /// Метод реализует контракт <see cref="IRequestContext.Apply"/>.
    /// Для основного аккаунта не требуется добавление специфичных параметров (таких как <c>friend_id</c>
    /// или <c>group_id</c>) — все необходимые данные подставляются автоматически при отправке запроса.
    /// <list type="bullet">
    /// <item><description>Подпись запроса (<c>sig</c>) рассчитывается после применения всех параметров.</description></item>
    /// <item><description>Учётные данные для подписи берутся из конфигурации <see cref="IOkApiClientCore"/>.</description></item>
    /// <item><description>Исходный экземпляр <paramref name="parameters"/> не модифицируется.</description></item>
    /// </list>
    /// </remarks>
    public RestParameters Apply(RestParameters parameters)
    {
        return parameters;
    }
}