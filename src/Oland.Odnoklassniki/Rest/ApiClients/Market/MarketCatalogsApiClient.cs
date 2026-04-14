using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Responses;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Response;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market;

/// <inheritdoc />
public class MarketCatalogsApiClient(IOkApiClientCore okApi) : IMarketCatalogsApiClient
{
    private const string OkClassName = "market";

    private const string AddCatalogMethodName = $"{OkClassName}.addCatalog";

    /// <inheritdoc />
    public async Task<string?> AddAsync(string title, bool adminRestricted, IRequestContext context, string? photoId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertName(title)
            .InsertPhotoId(photoId)
            .InsertAdminRestricted(adminRestricted);

        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<AddEntityResponse>(
            AddCatalogMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Id;
    }

    private const string EditCatalogMethodName = $"{OkClassName}.editCatalog";

    /// <inheritdoc />
    public async Task<bool> EditAsync(string catalogId, string title, bool adminRestricted, IRequestContext context, string? photoId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertName(title)
            .InsertCatalogId(catalogId)
            .InsertPhotoId(photoId)
            .InsertAdminRestricted(adminRestricted);

        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            EditCatalogMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    private const string DeleteCatalogMethodName = $"{OkClassName}.deleteCatalog";

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string catalogId, bool deleteProducts, IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCatalogId(catalogId)
            .InsertCustomParameter("delete_products", deleteProducts);
        
        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            DeleteCatalogMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    private const string ReorderCatalogMethodName = $"{OkClassName}.reorderCatalogs";

    /// <inheritdoc />
    public async Task<bool> ReorderAsync(string catalogId,
        IRequestContext context,
        string? afterCatalogId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCatalogId(catalogId)
            .InsertCustomParameter("after_catalog_id", afterCatalogId);
        
        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            ReorderCatalogMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    public AnchorNavigator<TCatalogDto> GetByGroupAnchorNavigator<TCatalogDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TCatalogDto : BaseOkDto
    {
        return new AnchorNavigator<TCatalogDto>(configuration => GetCatalogsByGroupAsync<TCatalogDto>(context, configuration, fields, cancellationToken), anchorConfiguration);
    }

    private const string GetCatalogsByGroupMethodName = $"{OkClassName}.getCatalogsByGroup";

    private async Task<AnchorResponse<TCatalogDto>> GetCatalogsByGroupAsync<TCatalogDto>(IRequestContext context, AnchorConfiguration anchorConfiguration, IEnumerable<string>? fields = null, CancellationToken cancellationToken = default) where TCatalogDto : BaseOkDto
    {
        fields ??= [CatalogBeanFields.UserId, CatalogBeanFields.Name, CatalogBeanFields.Capabilities];
        
        var parameters = new RestParameters()
            .InsertAnchor(anchorConfiguration.Anchor)
            .InsertDirection(anchorConfiguration.Direction)
            .InsertCount(anchorConfiguration.Count)
            .InsertFields(fields.ToArray());
        
        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<CatalogsResponse<TCatalogDto>>(
            GetCatalogsByGroupMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<TCatalogDto>()
        {
            Anchor = response.Anchor,
            HasMore = response.HasMore,
            TotalCount = response.TotalCount,
            Results = response.Catalogs
        };
    }

    private const string GetCatalogsByIdsMethodName = $"{OkClassName}.getCatalogsByIds";

    public async Task<ICollection<TCatalogDto>> GetByIdsAsync<TCatalogDto>(IEnumerable<string> catalogIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TCatalogDto : BaseOkDto
    {
        if (!catalogIds.Any())
        {
            return [];
        }
        
        fields ??= [CatalogBeanFields.UserId, CatalogBeanFields.Name, CatalogBeanFields.Capabilities];
        
        var parameters = new RestParameters()
            .InsertCatalogIds(catalogIds)
            .InsertFields(fields.ToArray());
        
        switch (context)
        {
            case GroupRequestContext or MainGroupRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext));
        }
        
        var response = await okApi.CallAsync<CatalogsResponse<TCatalogDto>>(
            GetCatalogsByIdsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response.Catalogs;
    }
}