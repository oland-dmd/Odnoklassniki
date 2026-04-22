using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Link;

public class LinkMedia : MediaItem
{
    [JsonPropertyName("url")] public required string Url { get; set; }
}