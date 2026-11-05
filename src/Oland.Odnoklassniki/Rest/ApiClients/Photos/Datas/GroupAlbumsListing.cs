namespace Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

/// <summary>A bounded group-album traversal. Partial observations never prove absence.</summary>
public sealed record GroupAlbumsListing(IReadOnlyList<AlbumData> Albums, bool IsComplete, string? FailureReason);
