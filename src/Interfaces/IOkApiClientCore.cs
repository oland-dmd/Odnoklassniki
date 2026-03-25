using Odnoklassniki.Rest;

namespace Odnoklassniki.Interfaces;

/// <summary>
/// Базовый контракт для выполнения подписанных запросов к методам API Одноклассников (OK.ru).
/// Обеспечивает унифицированный механизм вызова эндпоинтов с автоматической генерацией подписи (<c>sig</c>).
/// </summary>
/// <remarks>
/// Реализация интерфейса отвечает за:
/// <list type="bullet">
/// <item>Формирование параметров запроса согласно спецификации OK.ru;</item>
/// <item>Расчёт криптографической подписи на основе <c>session_secret_key</c> или <c>application_secret</c>;</item>
/// <item>Отправку HTTP-запросов через внутренний клиент <see cref="ApiClientCore.IOkApiCore"/>;</item>
/// <item>Десериализацию JSON-ответа в целевой тип или возврат «сырого» ответа.</item>
/// </list>
/// Поддерживает работу как с пользовательскими сессиями (OAuth), так и в режиме приложения.
/// </remarks>
public interface IOkApiClientCore
{
    /// <summary>
    /// Выполняет вызов метода OK.ru API с пользовательской сессией и возвращает десериализованный результат.
    /// </summary>
    /// <remarks>
    /// Автоматически формирует подпись запроса (<c>sig</c>) по алгоритму OK.ru:
    /// сортировка параметров, конкатенация с секретным ключом, вычисление MD5.
    /// При передаче <paramref name="accessToken"/> и <paramref name="secret"/> используется сессионный режим,
    /// иначе — режим приложения (требует корректной настройки <see cref="ApiClientCore.ApplicationOptions"/>).
    /// </remarks>
    /// <typeparam name="T">Тип результата для десериализации JSON-ответа.</typeparam>
    /// <param name="methodName">
    /// Полное имя метода API в формате <c>namespace.method</c>, например: <c>"users.getCurrentUser"</c>, <c>"auth.touchSession"</c>.
    /// Обязательный параметр.
    /// </param>
    /// <param name="accessToken">
    /// OAuth-токен пользователя. Обязателен для методов, требующих авторизации от имени пользователя.
    /// Если не указан, запрос выполняется в контексте приложения.
    /// </param>
    /// <param name="secret">
    /// Секретный ключ сессии (<c>session_secret_key</c>), полученный при авторизации.
    /// Используется для расчёта подписи. Если не указан, применяется <c>application_secret</c>.
    /// </param>
    /// <param name="parameters">
    /// Дополнительные параметры метода API (например, <c>uids</c>, <c>fields</c>, <c>count</c>).
    /// Передаются как коллекция <c>ключ → значение</c>. Может быть <see langword="null"/>.
    /// </param>
    /// <param name="markOnline">
    /// Если <see langword="true"/>, добавляет параметр <c>__online=true</c> для отметки пользователя как «онлайн».
    /// Работает только для приложений с соответствующими правами (мессенджеры, чаты).
    /// Значение <see langword="null"/> исключает параметр из запроса.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, представляющая асинхронную операцию с результатом типа <typeparamref name="T"/>.
    /// Возвращает <see langword="null"/> при пустом ответе или ошибке десериализации.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Если <paramref name="methodName"/> пуст или <paramref name="accessToken"/> указан, но невалиден.
    /// </exception>
    /// <exception cref="Refit.ApiException">
    /// При ошибках транспорта, неверной подписи (<c>INVALID_SIG</c>), или ошибках валидации на стороне OK.ru.
    /// </exception>
    Task<T?> CallAsync<T>(
        string methodName,
        string accessToken = "",
        string secret = "",
        RestParameters? parameters = null,
        bool? markOnline = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполняет вызов метода OK.ru API и возвращает «сырой» JSON-ответ в виде строки.
    /// </summary>
    /// <remarks>
    /// Предназначен для сценариев, где требуется ручная обработка ответа или работа с динамической структурой данных.
    /// Параметры подписи и авторизации обрабатываются аналогично перегрузке с дженерик-типом.
    /// </remarks>
    /// <param name="methodName">
    /// Полное имя метода API в формате <c>namespace.method</c>. Обязательный параметр.
    /// </param>
    /// <param name="accessToken">
    /// OAuth-токен пользователя. Необязателен для публичных методов.
    /// </param>
    /// <param name="secret">
    /// Секретный ключ сессии для расчёта подписи. При отсутствии используется секрет приложения.
    /// </param>
    /// <param name="parameters">Дополнительные параметры запроса. Может быть <see langword="null"/>.</param>
    /// <param name="markOnline">
    /// Флаг отметки пользователя как «онлайн». Добавляет параметр <c>__online</c> в запрос.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача с результатом в виде строки (JSON-ответ от API) или <see langword="null"/> при ошибке.
    /// </returns>
    /// <exception cref="Refit.ApiException">
    /// При ошибках сетевого взаимодействия или возврате статуса, отличного от 200 OK.
    /// </exception>
    Task<string?> CallAsync(
        string methodName,
        string accessToken = "",
        string secret = "",
        RestParameters? parameters = null,
        bool? markOnline = false,
        CancellationToken cancellationToken = default);
}