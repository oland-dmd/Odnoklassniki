namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Models;

public record PhotoModel
{
    public required string PhotoToken { get; init; }
    
    public bool IsValid => !string.IsNullOrWhiteSpace(PhotoToken);
}