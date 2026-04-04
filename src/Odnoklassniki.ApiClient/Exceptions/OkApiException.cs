using System.Text.Json;

namespace Odnoklassniki.Exceptions;

/// <summary>
/// Исключение, возникающее при получении ошибки от API Одноклассников.
/// Содержит код ошибки и текстовое описание, возвращённые сервером OK.ru.
/// </summary>
/// <remarks>
/// Используется для унифицированной обработки ошибок API во всех клиентах.
/// Код ошибки соответствует спецификации OK.ru (например, <c>100</c> — неверный параметр, <c>300</c> — неверная подпись).
/// Исключение выбрасывается автоматически при десериализации ответа, содержащего поле <c>error_code</c>.
/// </remarks>
public class OkApiException : Exception
{
    /// <summary>
    /// Числовой код ошибки согласно спецификации API Одноклассников.
    /// </summary>
    /// <value>
    /// Целочисленное значение, идентифицирующее тип ошибки.
    /// </value>
    /// <remarks>
    /// Примеры распространённых кодов:
    /// <list type="bullet">
    /// <item><c>100</c> — один из параметров запроса неверен;</item>
    /// <item><c>101</c> — метод API не найден;</item>
    /// <item><c>200</c> — недостаточно прав доступа;</item>
    /// <item><c>300</c> — неверная подпись запроса (INVALID_SIG);</item>
    /// <item><c>1000</c> — внутренняя ошибка сервера OK.ru.</item>
    /// </list>
    /// Полный список кодов доступен в официальной документации Одноклассников.
    /// </remarks>
    public int ErrorCode { get; }
    
    /// <summary>
    /// Инициализирует новый экземпляр исключения с сообщением и кодом ошибки.
    /// </summary>
    /// <param name="message">Текстовое описание ошибки от сервера API.</param>
    /// <param name="errorCode">Числовой код ошибки для программной обработки.</param>
    public OkApiException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Проверяет ответ API на наличие ошибки и выбрасывает исключение при её обнаружении.
    /// </summary>
    /// <remarks>
    /// Метод выполняет быструю проверку строки ответа на наличие подстроки <c>"error_code"</c>.
    /// При обнаружении выполняется десериализация в <see cref="ErrorJsonObject"/> и выбрасывается
    /// <see cref="OkApiException"/> с соответствующими параметрами.
    /// </remarks>
    /// <param name="response">
    /// «Сырой» JSON-ответ от сервера API Одноклассников.
    /// Может содержать как успешный ответ, так и объект ошибки.
    /// </param>
    /// <exception cref="OkApiException">
    /// Выбрасывается, если ответ содержит поле <c>error_code</c>.
    /// Исключение содержит код ошибки и сообщение от сервера.
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    /// Может возникнуть при некорректном формате JSON в ответе (редкий случай).
    /// </exception>
    public static void ThrowIfError(string response)
    {
        if (!response.Contains("error_code")) return;
        
        var errorJson = JsonSerializer.Deserialize<ErrorJsonObject>(response);

        throw new OkApiException(errorJson!.Message, errorJson.ErrorCode);
    }
}