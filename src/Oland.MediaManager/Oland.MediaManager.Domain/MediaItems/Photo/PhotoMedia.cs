using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Photo;

public class PhotoMedia : MediaItem
{
    [JsonPropertyName("list")] public List<PhotoItem> List { get; set; } = new();
}