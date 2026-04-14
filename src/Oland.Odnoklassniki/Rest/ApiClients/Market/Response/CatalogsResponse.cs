using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

//TODO: Вынести общее
public record CatalogsResponse<TCatalogDto>() where TCatalogDto : BaseOkDto
{
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }
    
    [JsonPropertyName("catalogs")]
    public ICollection<TCatalogDto> Catalogs { get; init; }
    
    [JsonPropertyName("Etag")]
    public string Etag { get; init; }
    
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
};