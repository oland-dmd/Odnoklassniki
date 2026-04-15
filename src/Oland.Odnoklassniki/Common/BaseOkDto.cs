using System.Text.Json;
using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Common;
/// <summary>
/// Базовый класс для всех DTO, получающих данные из ОК API.
/// Автоматически сохраняет все нераспознанные поля в ExtendedData.
/// </summary>
public abstract record BaseOkDto
{
    /// <summary>
    /// Динамические поля, не описанные в модели явно.
    /// Заполняется автоматически при десериализации.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtendedData { get; set; }

    /// <summary>
    /// Безопасное получение значения из расширенных данных
    /// </summary>
    public T? GetExtended<T>(string key, JsonSerializerOptions? options = null)
    {
        if (ExtendedData?.TryGetValue(key, out var element) == true)
        {
            return element.Deserialize<T>(options);
        }
        return default;
    }

    /// <summary>
    /// Проверка наличия динамического поля
    /// </summary>
    public bool HasExtended(string key) => ExtendedData?.ContainsKey(key) == true;
}