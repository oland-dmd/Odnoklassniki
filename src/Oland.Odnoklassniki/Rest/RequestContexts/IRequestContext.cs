using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Базовый контракт контекста запроса для работы с API Одноклассников (OK.ru).
/// Определяет единый интерфейс для передачи учётных данных и применения параметров,
/// специфичных для различных сценариев авторизации и целевых объектов запроса.
/// </summary>
/// <remarks>
/// Реализации интерфейса используются для инкапсуляции логики подготовки запросов:
/// <list type="bullet">
/// <item><description><see cref="ExplicitTokenRequestContext"/> — запросы с явной передачей токенов;</description></item>
/// <item><description><see cref="FriendRequestContext"/> — получение данных со страницы друга;</description></item>
/// <item><description><see cref="GroupRequestContext"/> — работа с группами и их метаданными.</description></item>
/// </list>
/// Контекст не выполняет сетевые запросы самостоятельно — он подготавливает данные
/// для отправки через ядро клиента <see cref="IOkApiClientCore"/>.
/// <list type="bullet">
/// <item><description>Все реализации должны быть неизменяемыми (immutable) для обеспечения потокобезопасности.</description></item>
/// <item><description>Учётные данные в свойстве <see cref="AccessPair"/> являются конфиденциальными — не передавайте их в логах или ответах.</description></item>
/// <item><description>Метод <see cref="Apply"/> не должен модифицировать входной экземпляр <see cref="RestParameters"/> — возвращайте новый объект.</description></item>
/// </list>
/// </remarks>
public interface IRequestContext
{
    /// <summary>
    /// Пара учётных данных для авторизации запроса: токен доступа и секрет сессии.
    /// </summary>
    /// <remarks>
    /// Содержит чувствительные данные, необходимые для расчёта криптографической подписи (<c>sig</c>)
    /// и выполнения методов от имени пользователя. Может быть пустым (значения по умолчанию),
    /// если запрос выполняется в режиме приложения или с использованием глобальных настроек клиента.
    /// <list type="bullet">
    /// <item><description><see cref="AccessPair.AccessToken"/> — OAuth-токен, полученный при авторизации.</description></item>
    /// <item><description><see cref="AccessPair.SessionSecretKey"/> — секрет сессии для подписи запросов (конфиденциально).</description></item>
    /// <item><description>При компрометации данных необходимо немедленно отозвать сессию через <c>auth.logout</c>.</description></item>
    /// </list>
    /// </remarks>
    AccessPair AccessPair { get; }

    /// <summary>
    /// Применяет контекст к параметрам запроса, добавляя специфичные для сценария значения.
    /// </summary>
    /// <param name="parameters">Исходные параметры запроса типа <see cref="RestParameters"/>.</param>
    /// <returns>
    /// Новый или модифицированный экземпляр <see cref="RestParameters"/> с применёнными
    /// параметрами контекста (например, <c>friend_id</c>, <c>group_id</c> или другими).
    /// </returns>
    /// <remarks>
    /// Метод вызывается ядром клиента перед расчётом подписи и отправкой запроса.
    /// Реализации должны:
    /// <list type="bullet">
    /// <item><description>Не модифицировать входной экземпляр <paramref name="parameters"/> — возвращать новый объект или использовать immutable-подход.</description></item>
    /// <item><description>Корректно кодировать добавляемые значения в соответствии с требованиями подписи OK.ru.</description></item>
    /// <item><description>Перезаписывать существующие параметры с теми же именами, если это требуется логикой контекста.</description></item>
    /// </list>
    /// Подпись запроса (<c>sig</c>) рассчитывается после применения всех параметров на уровне <see cref="IOkApiClientCore"/>.
    /// </remarks>
    RestParameters Apply(RestParameters parameters);
}