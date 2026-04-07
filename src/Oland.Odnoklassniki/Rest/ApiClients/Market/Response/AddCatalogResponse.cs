using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

public class AddCatalogResponse
{
    [JsonPropertyName("catalog_id")]
    public string? CatalogId { get; init; }
}