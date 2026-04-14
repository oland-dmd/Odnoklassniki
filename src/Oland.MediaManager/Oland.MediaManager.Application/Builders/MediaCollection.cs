using System.Text.Json;
using System.Text.Json.Serialization;
using Oland.MediaManager.Domain.MediaItems;

namespace Oland.MediaManager.Application.Builders;

/// <summary>
///     Обёртка над результатом сборки
/// </summary>
public class MediaCollection
{
    internal MediaCollection(List<MediaItem> items)
    {
        Items = items;
    }

    [JsonPropertyName("media")] public List<MediaItem> Items { get; }

    public static JsonSerializerOptions DefaultOptions => new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string ToJson(JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(new { media = Items }, options ?? DefaultOptions);
    }
}