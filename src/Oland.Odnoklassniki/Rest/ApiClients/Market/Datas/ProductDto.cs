using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;

public record ProductDto() : BaseOkDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("group_photo")]
    public string? GroupPhoto { get; init; }

    [JsonPropertyName("image_url_base")]
    public string? ImageUrlBase { get; init; }
}