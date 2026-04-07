namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;

public record CreatedCatalogData
{
    public required string CatalogId { get; init; }
    public bool Success { get; init; }
}