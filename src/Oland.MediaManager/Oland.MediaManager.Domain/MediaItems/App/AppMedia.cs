using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.App;

public class AppMedia : MediaItem
{
    public override string Type => "app";

    [JsonPropertyName("text")] public string? Text { get; set; }

    [JsonPropertyName("images")] public List<AppImage> Images { get; set; } = new();

    [JsonPropertyName("actions")] public List<AppAction> Actions { get; set; } = new();
}