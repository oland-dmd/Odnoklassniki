using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Movie;

public class MovieMedia : MediaItem
{
    public override string Type => "movie";

    [JsonPropertyName("list")] public List<MovieItem> List { get; set; } = new();
}