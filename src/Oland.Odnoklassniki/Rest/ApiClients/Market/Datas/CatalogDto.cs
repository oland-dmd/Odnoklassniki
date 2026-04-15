using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;

public record CatalogDto : BaseOkDto
{
    private string _ref;
    
    [JsonPropertyName("ref")]
    public string Id
    {
        get => _ref.Split(':').Last();
        set => _ref = value;
    }

    [JsonPropertyName("name")]
    public string? Title { get; set; }
}