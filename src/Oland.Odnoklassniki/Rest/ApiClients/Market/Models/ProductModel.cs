namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Models;

public record ProductModel
{
    public required string Title { get; init; }
    
    public string? Description { get; init; }
    
    public ICollection<PhotoModel>? Photos {get; init;}
    
    public int LifetimePerDays { get; init; }
    
    public required decimal Price {get; init;}

    public string? Currency { get; init; }
    
    public string? PartnerLink { get; init; }
    
    public bool IsValid => 
        (!string.IsNullOrWhiteSpace(Description) || Photos?.Count > 0)
        && (Photos?.All(p => p.IsValid) ?? true)
        && !string.IsNullOrWhiteSpace(Title)
        && LifetimePerDays >= 0
        && Price >= 0;
}