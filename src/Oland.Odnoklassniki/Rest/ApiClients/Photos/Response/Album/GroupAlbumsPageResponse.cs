using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Response.Album;

internal sealed record GroupAlbumsPageResponse
{
    [JsonPropertyName("albums")]
    public AlbumModel?[]? Albums { get; init; }

    [JsonPropertyName("hasMore")]
    public bool? HasMore { get; init; }

    [JsonPropertyName("pagingAnchor")]
    public string? Anchor { get; init; }
}
