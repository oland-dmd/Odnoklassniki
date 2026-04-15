using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Movie;

public class MovieItem
{
    [JsonPropertyName("id")] public required string Id { get; set; }
}