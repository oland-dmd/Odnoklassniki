using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Response;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market;

/// <inheritdoc />
public class CatalogsApiClient(IOkApiClientCore okApi) : ICatalogsApiClient
{
    private const string OkClassName = "market";

    private const string AddCatalogMethodName = $"{OkClassName}.addCatalog";

    /// <inheritdoc />
    public async Task<string?> AddCatalogAsync(string title, string photoId, bool adminRestricted, IRequestContext context,
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
        
        var response = await okApi.CallAsync<AddCatalogResponse>(
            AddCatalogMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.CatalogId;
    }

    private const string EditCatalogMethodName = $"{OkClassName}.editCatalog";

    /// <inheritdoc />
    public async Task<bool> EditCatalogAsync(string catalogId, string title, string photoId, bool adminRestricted, IRequestContext context,
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
    public async Task<bool> DeleteCatalogAsync(string catalogId, bool deleteProducts, IRequestContext context,
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

    private const string ReorderCatalogMethodName = $"{OkClassName}.reorderCatalog";

    /// <inheritdoc />
    public async Task<bool> ReorderCatalogAsync(string catalogId, string afterCatalogId, IRequestContext context,
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
}