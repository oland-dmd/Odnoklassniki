using Refit;

namespace Odnoklassniki.Rest.ApiClientCore;

/// <summary>
/// Внутренний интерфейс базового клиента для HTTP-запросов к API Одноклассников.
/// Реализован через библиотеку Refit для автоматической генерации прокси-клиента.
/// </summary>
/// <remarks>
/// Используется как основа для специализированных API-клиентов.
/// Все запросы выполняются через метод <c>fb.do</c> с динамической подстановкой пути.
/// </remarks>
internal interface IOkApiCore
{
    /// <summary>
    /// Выполняет GET-запрос к указанному эндпоинту API.
    /// </summary>
    /// <remarks>
    /// URL формируется путём подстановки параметра <c>url</c> в шаблон <c>/fb.do?/{url}</c>.
    /// Пример: при <c>url="users.getInfo"</c> итоговый запрос будет <c>/fb.do?/users.getInfo</c>.
    /// </remarks>
    /// <typeparam name="T">Тип десериализуемого ответа.</typeparam>
    /// <param name="url">Относительный путь к методу API (без начального слэша).</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Десериализованный ответ типа <typeparamref name="T"/>.</returns>
    [Get("/fb.do?/{url}")]
    Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default);
}