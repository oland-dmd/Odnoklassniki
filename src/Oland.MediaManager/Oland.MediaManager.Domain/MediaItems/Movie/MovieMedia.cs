using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Movie;

public class MovieMedia : MediaItem
{
    [JsonPropertyName("list")] public List<MovieItem> List { get; set; } = new();
}