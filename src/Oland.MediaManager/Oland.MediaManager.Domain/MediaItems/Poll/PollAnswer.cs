using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Poll;

public class PollAnswer
{
    [JsonPropertyName("text")] public required string Text { get; set; }
}