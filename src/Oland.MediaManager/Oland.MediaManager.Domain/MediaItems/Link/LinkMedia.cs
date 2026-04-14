using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Link;

public class LinkMedia : MediaItem
{
    public override string Type => "link";

    [JsonPropertyName("url")] public required string Url { get; set; }
}