using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

//TODO: Вынести общее
public record ProductsResponse<TDto>() where TDto : BaseOkDto
{
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }
    
    [JsonPropertyName("products")]
    public ICollection<TDto> Products { get; init; }
    
    [JsonPropertyName("Etag")]
    public string Etag { get; init; }
    
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
    
    public bool Inconsistent { get; init; }
}