using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Photo;

public class PhotoMedia : MediaItem
{
    public override string Type => "photo";

    [JsonPropertyName("list")] public List<PhotoItem> List { get; set; } = new();
}