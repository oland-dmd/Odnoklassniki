using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Text;

public class TextMedia : MediaItem
{
    public override string Type => "text";

    [JsonPropertyName("text")] public required string Text { get; set; }
}