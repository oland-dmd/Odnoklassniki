using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Music;

public class MusicMedia : MediaItem
{
    public override string Type => "music";

    [JsonPropertyName("list")] public List<MusicItem> List { get; set; } = new();
}