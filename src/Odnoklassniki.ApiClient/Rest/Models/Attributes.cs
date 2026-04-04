using System.Text.Json.Serialization;
using Odnoklassniki.Rest.ApiClients.Photos.Response.Album;

namespace Odnoklassniki.Rest.Models;

/// <summary>
/// Модель дополнительных атрибутов альбома фотографий в API Одноклассников.
/// Содержит флаги и метаданные, определяющие поведение и настройки альбома.
/// </summary>
/// <remarks>
/// Класс является вспомогательной моделью для десериализации поля <c>attrs</c>
/// в ответе методов работы с альбомами. Используется внутри <see cref="AlbumModel"/>.
/// 
/// <para><b>Пример ответа сервера:</b></para>
/// <code>
/// {
///   "attrs": {
///     "flags": "ap"
///   }
/// }
/// </code>
/// 
/// <para><b>Интерпретация флагов:</b></para>
/// Значение поля <c>flags</c> представляет собой битовую маску или строковое представление
/// состояния альбома. Конкретная интерпретация зависит от версии API и контекста запроса.
/// </remarks>
public class Attributes
{
    /// <summary>
    /// Строковое представление флагов альбома. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>flags</c> в объекте <c>attrs</c> ответа API.
    /// Содержит информацию о настройках альбома в закодированном виде.
    /// </remarks>
    /// <value>
    /// Строковое значение флагов. Не может быть пустым или <see langword="null"/>.
    /// </value>
    [JsonPropertyName("flags")]
    public required string Flags { get; init; }
}