using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Music;

public class MusicItem
{
    [JsonPropertyName("id")] public required string Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("artistName")] public string? ArtistName { get; set; }

    [JsonPropertyName("albumName")] public string? AlbumName { get; set; }
}