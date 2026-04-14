using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.App;

public class AppAction
{
    [JsonPropertyName("text")] public required string Text { get; set; }

    [JsonPropertyName("mark")] public string? Mark { get; set; }
}