using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Poll;

public class PollMedia : MediaItem
{
    public override string Type => "poll";

    [JsonPropertyName("question")] public required string Question { get; set; }

    [JsonPropertyName("answers")] public List<PollAnswer> Answers { get; set; } = new();

    [JsonPropertyName("options")] public string? Options { get; set; } // "SingleChoice,AnonymousVoting"
}