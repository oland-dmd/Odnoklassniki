using System.Text.Json;
using System.Text.Json.Serialization;

namespace Oland.MediaManager.Application.Options;

/// <summary>
/// Настройки медиа-сервиса
/// </summary>
public class MediaServiceOptions
{
    /// <summary>
    /// Включить автоматическую валидацию при сериализации (по умолчанию: true)
    /// </summary>
    public bool AutoValidate { get; set; } = true;

    /// <summary>
    /// Использовать camelCase для имён свойств в JSON
    /// </summary>
    public bool UseCamelCase { get; set; } = true;

    /// <summary>
    /// Форматировать вывод JSON (для отладки)
    /// </summary>
    public bool WriteIndented { get; set; } = false;

    /// <summary>
    /// Игнорировать null-значения при сериализации
    /// </summary>
    public bool IgnoreNullValues { get; set; } = true;

    public JsonSerializerOptions ToJsonOptions() => new()
    {
        PropertyNamingPolicy = UseCamelCase ? JsonNamingPolicy.CamelCase : null,
        WriteIndented = WriteIndented,
        DefaultIgnoreCondition = IgnoreNullValues 
            ? JsonIgnoreCondition.WhenWritingNull 
            : JsonIgnoreCondition.Never
    };
}