using System.Text.Json.Serialization;

#pragma warning disable CS8618

namespace Odnoklassniki.Image;

/// <summary>
/// DTO для представления ответа сервера при загрузке изображения в API Одноклассников.
/// Содержит коллекцию токенов загруженных фотографий для последующего коммита.
/// </summary>
/// <remarks>
/// Используется для десериализации JSON-ответа от эндпоинта загрузки изображений
/// (полученного через метод API <c>photos.getUploadUrl</c>).
/// 
/// <para><b>Пример ответа сервера:</b></para>
/// <code>
/// {
///   "photos": {
///     "pic0": { "token": "abc123xyz..." },
///     "pic1": { "token": "def456uvw..." }
///   }
/// }
/// </code>
/// 
/// <para><b>Сценарий использования:</b></para>
/// <list type="number">
/// <item>Получить URL загрузки через <c>photos.getUploadUrl</c>;</item>
/// <item>Загрузить изображение через <see cref="ImageClient.UploadImageAsync"/>;</item>
/// <item>Извлечь токены из свойства <see cref="Photos"/>;</item>
/// <item>Передать токены в метод <c>photos.commit</c> для публикации фотографий.</item>
/// </list>
/// </remarks>
public record UploadResponse
{
    /// <summary>
    /// Словарь загруженных фотографий с ключами-идентификаторами полей формы и соответствующими токенами.
    /// </summary>
    /// <remarks>
    /// Ключи словаря соответствуют именам полей multipart/form-data запроса (например, <c>pic0</c>, <c>pic1</c>).
    /// Значения содержат токены (<see cref="TokenInfo"/>), необходимые для финализации загрузки.
    /// 
    /// <para><b>Формат для коммита:</b></para>
    /// Токены передаются в метод <c>photos.commit</c> в виде строки:
    /// <c>pic0:token1,pic1:token2</c>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Количество записей соответствует числу загруженных изображений в запросе;</item>
    /// <item>Порядок ключей должен сохраняться при формировании параметра коммита;</item>
    /// <item>Токены действительны ограниченное время (рекомендуется немедленный коммит).</item>
    /// </list>
    /// </remarks>
    /// <value>
    /// Коллекция пар <c>ключ → токен</c> для всех загруженных в запросе изображений.
    /// Может быть пустой, если загрузка не выполнена, но не должен быть <see langword="null"/> при успешном ответе.
    /// </value>
    [JsonPropertyName("photos")]
    public Dictionary<string, TokenInfo> Photos { get; init; }
}