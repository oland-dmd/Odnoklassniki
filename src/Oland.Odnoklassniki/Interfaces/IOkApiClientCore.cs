using Oland.Odnoklassniki.Rest;

namespace Oland.Odnoklassniki.Interfaces;

/// <summary>
/// Базовый контракт для выполнения подписанных запросов к методам API Одноклассников (OK.ru).
/// Обеспечивает унифицированный механизм вызова эндпоинтов с автоматической генерацией подписи (<c>sig</c>)
/// согласно спецификации платформы. Все запросы направляются во внешнюю систему OK.ru.
/// </summary>
/// <remarks>
/// Реализация интерфейса отвечает за:
/// <list type="bullet">
/// <item><description>Формирование параметров запроса согласно спецификации OK.ru;</description></item>
/// <item><description>Расчёт криптографической подписи на основе <c>session_secret_key</c> или <c>application_secret</c> (алгоритм MD5);</description></item>
/// <item><description>Отправку HTTP-запросов и обработку транспортных ошибок;</description></item>
/// <item><description>Десериализацию JSON-ответа в целевой тип или возврат «сырого» ответа.</description></item>
/// </list>
/// Поддерживает два режима работы: пользовательская сессия (OAuth) и режим приложения.
/// При ошибке валидации подписи сервер возвращает <c>INVALID_SIG</c> — в этом случае выбрасывается <see cref="Refit.ApiException"/>.
/// </remarks>
public interface IOkApiClientCore
{
    /// <summary>
    /// Выполняет вызов метода OK.ru API с автоматическим расчётом подписи и возвращает десериализованный результат.
    /// </summary>
    /// <typeparam name="T">Тип результата для десериализации JSON-ответа. Должен соответствовать структуре ответа метода API.</typeparam>
    /// <param name="methodName">
    /// Полное имя метода API в формате <c>namespace.method</c> (например, <c>"users.getCurrentUser"</c>, <c>"auth.touchSession"</c>).
    /// Обязательный параметр. Регистр символов должен точно соответствовать документации OK.ru.
    /// </param>
    /// <param name="accessToken">
    /// OAuth-токен пользователя. Обязателен для методов, требующих авторизации от имени пользователя.
    /// Если передана пустая строка или <see langword="null"/>, запрос выполняется в контексте приложения
    /// с использованием <c>application_secret</c> из настроек.
    /// </param>
    /// <param name="secret">
    /// Секретный ключ сессии (<c>session_secret_key</c>), полученный при авторизации.
    /// Используется для расчёта подписи. Если не указан, применяется <c>application_secret</c>.
    /// Не передавайте этот параметр в логах или ответах клиенту — он является конфиденциальным.
    /// </param>
    /// <param name="parameters">
    /// Дополнительные параметры метода API (например, <c>uids</c>, <c>fields</c>, <c>count</c>).
    /// Передаются как коллекция <c>ключ → значение</c>. Может быть <see langword="null"/> для запросов без параметров.
    /// Ключи и значения автоматически кодируются в соответствии с требованиями подписи.
    /// </param>
    /// <param name="markOnline">
    /// Если <see langword="true"/>, добавляет параметр <c>__online=true</c> для отметки пользователя как «онлайн».
    /// Работает только для приложений с соответствующими правами (мессенджеры, чаты).
    /// Значение <see langword="null"/> исключает параметр из запроса.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, представляющая асинхронную операцию с результатом типа <typeparamref name="T"/>.
    /// Возвращает <see langword="default"/> при пустом ответе, ошибке десериализации или отсутствии данных.
    /// </returns>
    /// <remarks>
    /// Алгоритм формирования подписи:
    /// <list type="number">
    /// <item><description>Параметры запроса сортируются по ключу в лексикографическом порядке;</description></item>
    /// <item><description>Значения конкатенируются в строку формата <c>key1=value1key2=value2...</c>;</description></item>
    /// <item><description>К строке добавляется секретный ключ (<c>secret</c> или <c>application_secret</c>);</description></item>
    /// <item><description>Вычисляется MD5-хеш результата, который передаётся в параметре <c>sig</c>.</description></item>
    /// </list>
    /// <list type="bullet">
    /// <item><description>При использовании пользовательского токена убедитесь, что он не истёк — иначе сервер вернёт ошибку авторизации.</description></item>
    /// <item><description>Метод не кэширует ответы: каждый вызов выполняет сетевой запрос к OK.ru.</description></item>
    /// <item><description>Для массовых запросов реализуйте внешнее кэширование и соблюдение rate-limit платформы.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// Если <paramref name="methodName"/> пуст, содержит недопустимые символы или формат имени не соответствует спецификации.
    /// </exception>
    /// <exception cref="Refit.ApiException">
    /// При ошибках транспорта, неверной подписи (<c>INVALID_SIG</c>), истечении токена (<c>EXPIRED_TOKEN</c>)
    /// или других ошибках валидации на стороне OK.ru. Содержит код ошибки и текст ответа сервера.
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
    /// <param name="methodName">
    /// Полное имя метода API в формате <c>namespace.method</c>. Обязательный параметр.
    /// </param>
    /// <param name="accessToken">
    /// OAuth-токен пользователя. Необязателен для публичных методов.
    /// При отсутствии запрос выполняется в режиме приложения.
    /// </param>
    /// <param name="secret">
    /// Секретный ключ сессии для расчёта подписи. При отсутствии используется <c>application_secret</c>.
    /// </param>
    /// <param name="parameters">Дополнительные параметры запроса. Может быть <see langword="null"/>.</param>
    /// <param name="markOnline">
    /// Флаг отметки пользователя как «онлайн». Добавляет параметр <c>__online</c> в запрос.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача с результатом в виде строки (необработанный JSON-ответ от API)
    /// или <see langword="null"/> при ошибке транспорта, пустом ответе или ошибке десериализации.
    /// </returns>
    /// <remarks>
    /// Предназначен для сценариев, где требуется:
    /// <list type="bullet">
    /// <item><description>Ручная обработка ответа (например, при работе с динамической структурой данных);</description></item>
    /// <item><description>Логирование «сырого» ответа для отладки;</description></item>
    /// <item><description>Обход стандартной десериализации при несовместимости типов.</description></item>
    /// </list>
    /// Подпись и параметры авторизации обрабатываются аналогично дженерик-версии метода.
    /// </remarks>
    /// <exception cref="Refit.ApiException">
    /// При ошибках сетевого взаимодействия, неверной подписи или возврате статуса, отличного от 200 OK.
    /// </exception>
    Task<string?> CallAsync(
        string methodName,
        string accessToken = "",
        string secret = "",
        RestParameters? parameters = null,
        bool? markOnline = false,
        CancellationToken cancellationToken = default);
}