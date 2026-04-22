using System.Text.Json.Serialization;

namespace Oland.MediaManager.Domain.MediaItems.Text;

public class ProductMedia : MediaItem
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("lifetime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Lifetime { get; set; }
    
    [JsonPropertyName("partner_link")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PartnerLink { get; set; }
    
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Currency { get; set; }
}