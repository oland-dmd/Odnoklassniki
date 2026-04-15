using System.Text.Json;
using System.Text.Json.Serialization;
using Oland.MediaManager.Domain.MediaItems;

namespace Oland.MediaManager.Application.Builders;

/// <summary>
/// Обёртка над результатом сборки коллекции медиа-элементов для последующей сериализации.
/// </summary>
public class MediaCollection
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MediaCollection"/>.
    /// </summary>
    /// <param name="items">Список медиа-элементов для включения в коллекцию.</param>
    internal MediaCollection(List<MediaItem> items)
    {
        Items = items;
    }

    /// <summary>
    /// Коллекция медиа-элементов, сериализуемая в JSON-поле «media».
    /// </summary>
    [JsonPropertyName("media")] public List<MediaItem> Items { get; }

    /// <summary>
    /// Настройки сериализации JSON по умолчанию:
    /// - форматированный вывод;
    /// - пропуск значений null;
    /// - camelCase для имён свойств.
    /// </summary>
    public static JsonSerializerOptions DefaultOptions => new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Сериализует коллекцию в JSON-строку.
    /// </summary>
    /// <param name="options">
    /// Опции сериализации. Если не указаны, используются <see cref="DefaultOptions"/>.
    /// </param>
    /// <returns>JSON-представление коллекции в формате { "media": [...] }.</returns>
    public string ToJson(JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(new { media = Items }, options ?? DefaultOptions);
    }
}