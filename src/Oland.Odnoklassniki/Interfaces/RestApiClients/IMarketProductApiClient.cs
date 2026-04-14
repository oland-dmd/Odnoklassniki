using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Models;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

public interface IMarketProductsApiClient
{
    Task<string?> AddAsync(
        ProductModel model,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    Task<bool> EditAsync(
        string productId,
        ProductModel model,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(
        string productId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    AnchorNavigator<TDto> GetByCatalogNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;
    
    AnchorNavigator<TDto> GetProductsNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        bool onModeration = false,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;
    
    Task<ICollection<TDto>> GetByIdsAsync<TDto>(
        ICollection<string> productIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;
    
    Task<bool> PinAsync(
        string productId,
        bool on,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    Task<bool> ReorderAsync(
        string productId,
        IRequestContext context,
        string? afterProductId = null,
        CancellationToken cancellationToken = default);
    
    
}