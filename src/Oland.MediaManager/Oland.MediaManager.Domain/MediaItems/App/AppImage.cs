using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.App;

public class AppImage
{
    [JsonPropertyName("url")] public required string Url { get; set; }

    [JsonPropertyName("mark")] public string? Mark { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }
}