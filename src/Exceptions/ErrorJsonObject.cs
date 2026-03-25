using System.Text.Json.Serialization;

namespace Odnoklassniki.Exceptions;

/// <summary>
/// Модель ошибки, возвращаемой API Одноклассников при неудачных запросах.
/// Используется для десериализации JSON-ответов с кодом и описанием ошибки.
/// </summary>
/// <remarks>
/// Соответствует формату ошибок OK.ru:
/// <code>
/// {
///   "error_code": 100,
///   "error_msg": "One of the parameters is wrong"
/// }
/// </code>
/// Применяется в обработчиках исключений для парсинга ответов с HTTP-статусами 4xx/5xx.
/// </remarks>
public class ErrorJsonObject
{
    /// <summary>
    /// Числовой код ошибки согласно спецификации API Одноклассников.
    /// </summary>
    /// <remarks>
    /// Примеры кодов:
    /// <list type="bullet">
    /// <item><c>100</c> — один из параметров неверен;</item>
    /// <item><c>101</c> — метод не найден;</item>
    /// <item><c>200</c> — нет прав доступа;</item>
    /// <item><c>300</c> — неверная подпись запроса (INVALID_SIG).</item>
    /// </list>
    /// Полный список кодов доступен в документации OK.ru.
    /// </remarks>
    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }
    
    /// <summary>
    /// Текстовое описание ошибки на русском языке.
    /// </summary>
    /// <remarks>
    /// Содержит человеко-читаемое объяснение причины ошибки.
    /// Может использоваться для логирования или отображения пользователю.
    /// Не рекомендуется полагаться на точный текст сообщения — он может измениться.
    /// </remarks>
    [JsonPropertyName("error_msg")]
    public string Message { get; set; }
}