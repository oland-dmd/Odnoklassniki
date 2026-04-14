using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

public record ShortProductsResponse<TDto>() : ProductsResponse<TDto> where TDto : BaseOkDto
{
    [JsonPropertyName("short_products")]
    public new ICollection<TDto> Products { get; init; }
}