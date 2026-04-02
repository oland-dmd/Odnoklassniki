using Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Контекст запроса с явной передачей учётных данных (токен доступа и секрет сессии).
/// Реализует <see cref="IRequestContext"/> для сценариев, где авторизация выполняется
/// не через глобальные настройки клиента, а через непосредственную передачу пары
/// <see cref="AccessPair"/> в каждом запросе.
/// </summary>
/// <remarks>
/// Используется для работы с несколькими пользовательскими сессиями в рамках одного
/// экземпляра клиента, а также для выполнения запросов от имени разных пользователей
/// без пересоздания основного клиента <see cref="IOkApiClientCore"/>.
/// <list type="bullet">
/// <item><description>Учётные данные хранятся в неизменяемом свойстве <see cref="AccessPair"/>.</description></item>
/// <item><description>Метод <see cref="Apply"/> не модифицирует параметры запроса — подпись рассчитывается на уровне ядра клиента.</description></item>
/// <item><description>Не передавайте экземпляры этого класса в логах или ответах — они содержат конфиденциальные данные.</description></item>
/// </list>
/// </remarks>
public record ExplicitTokenRequestContext : IRequestContext
{
    /// <summary>
    /// Пара учётных данных для авторизации: токен доступа и секрет сессии.
    /// </summary>
    /// <remarks>
    /// Значение инициализируется в конструкторе и не может быть изменено после создания экземпляра.
    /// Используется ядром клиента для расчёта криптографической подписи (<c>sig</c>) запросов к API ОК.ру.
    /// <list type="bullet">
    /// <item><description><see cref="AccessPair.AccessToken"/> — OAuth-токен пользователя, полученный при авторизации.</description></item>
    /// <item><description><see cref="AccessPair.SessionSecretKey"/> — секрет сессии для подписи запросов (конфиденциально).</description></item>
    /// </list>
    /// </remarks>
    public AccessPair AccessPair { get; }

    /// <summary>
    /// Инициализирует новый экземпляр контекста с указанными учётными данными.
    /// </summary>
    /// <param name="accessPair">
    /// Пара <see cref="AccessPair"/>, содержащая токен доступа и секрет сессии.
    /// Обязательный параметр. Значения должны быть получены в результате успешной авторизации
    /// через метод <c>oauth.authorize</c> или аналогичный поток аутентификации ОК.ру.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// Если переданное значение <paramref name="accessPair"/> является <see langword="default"/>.
    /// </exception>
    public ExplicitTokenRequestContext(AccessPair accessPair)
    {
        AccessPair = accessPair;
    }

    /// <summary>
    /// Деконструирует контекст на отдельные компоненты учётных данных.
    /// </summary>
    /// <param name="accessToken">
    /// Возвращает токен доступа (<c>AccessPair.AccessToken</c>) для использования в запросах.
    /// </param>
    /// <param name="sessionSecretKey">
    /// Возвращает секрет сессии (<c>AccessPair.SessionSecretKey</c>) для расчёта подписи.
    /// </param>
    /// <remarks>
    /// Позволяет использовать паттерн деконструкции для извлечения учётных данных:
    /// <code>
    /// var (token, secret) = requestContext;
    /// </code>
    /// Не копируйте и не сохраняйте извлечённые значения вне контекста выполнения запроса.
    /// </remarks>
    public void Deconstruct(out string accessToken, out string sessionSecretKey)
    {
        accessToken = AccessPair.AccessToken;
        sessionSecretKey = AccessPair.SessionSecretKey;
    }

    /// <summary>
    /// Применяет контекст к параметрам запроса. В данной реализации не модифицирует входные данные.
    /// </summary>
    /// <param name="parameters">Исходные параметры запроса типа <see cref="RestParameters"/>.</param>
    /// <returns>
    /// Тот же экземпляр <see cref="RestParameters"/>, без изменений.
    /// Учётные данные для подписи используются на уровне ядра клиента, а не через этот метод.
    /// </returns>
    /// <remarks>
    /// Метод реализует контракт <see cref="IRequestContext.Apply"/>.
    /// Подпись запроса (<c>sig</c>) и параметры авторизации добавляются автоматически
    /// при отправке запроса через <see cref="IOkApiClientCore.CallAsync{T}"/>.
    /// </remarks>
    public RestParameters Apply(RestParameters parameters)
    {
        return parameters;
    }
}