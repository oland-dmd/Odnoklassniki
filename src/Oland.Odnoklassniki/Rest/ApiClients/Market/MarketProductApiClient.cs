using Oland.MediaManager.Application.Services;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Responses;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Constants;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Models;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Response;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market;

public class MarketProductsApiClient(IOkApiClientCore okApi, IMediaService mediaService) : IMarketProductsApiClient
{
    private const string OkClassName = "market";

    private const string AddMethodName = $"{OkClassName}.add";

    /// <inheritdoc />
    public async Task<string?> AddAsync(ProductModel model, IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (!model.IsValid)
        {
            throw new ArgumentException($"{nameof(model)} is not valid", nameof(model));
        }

        var parameters = new RestParameters();
        parameters = context switch
        {
            GroupCatalogsRequestContext or GroupRequestContext or MainGroupRequestContext => context.Apply(parameters)
                .InsertCustomParameter("type", "GROUP_PRODUCT"),
            MainAccountRequestContext or ExplicitTokenRequestContext => context.Apply(parameters)
                .InsertCustomParameter("type", "USER_PRODUCT"),
            _ => throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                nameof(MainGroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext),
                nameof(GroupCatalogsRequestContext))
        };

        var jsonAttachment = mediaService.CreateAndSerialize(builder =>
        {
            builder
                .AddText(model.Title)
                .AddText(model.Description)
                .AddPhoto(photoBuilder =>
                {
                    foreach (var photo in model.Photos ?? [])
                    {
                        photoBuilder.AddById(photo.PhotoToken);
                    }
                })
                .AddProduct(model.Price, model.PartnerLink, model.LifetimePerDays, model.Currency);
        }, true);
        
        parameters = parameters.InsertAttachment(jsonAttachment);
        
        var response = await okApi.CallAsync<AddEntityResponse>(
            AddMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Id;
    }
    
    private const string EditMethodName = $"{OkClassName}.edit";

    /// <inheritdoc />
    public async Task<bool> EditAsync(string productId, ProductModel model, IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (!model.IsValid)
        {
            throw new ArgumentException($"{nameof(model)} is not valid", nameof(model));
        }

        var parameters = new RestParameters();
        parameters = context switch
        {
            GroupCatalogsRequestContext or GroupRequestContext or MainGroupRequestContext or MainAccountRequestContext
                or ExplicitTokenRequestContext => context.Apply(parameters),
            _ => throw new UnexpectedRequestContext(context, nameof(GroupRequestContext),
                nameof(MainGroupRequestContext), nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext),
                nameof(GroupCatalogsRequestContext))
        };

        var jsonAttachment = mediaService.CreateAndSerialize(builder =>
        {
            builder
                .AddText(model.Title)
                .AddText(model.Description)
                .AddPhoto(photoBuilder =>
                {
                    foreach (var photo in model.Photos ?? [])
                    {
                        photoBuilder.AddById(photo.PhotoToken);
                    }
                })
                .AddProduct(model.Price, model.PartnerLink, model.LifetimePerDays, model.Currency);
        }, true);

        parameters = parameters.InsertAttachment(jsonAttachment)
            .InsertProductId(productId);

        var response = await okApi.CallAsync<CompletionStatusResponse>(
            EditMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    private const string DeleteMethodName = $"{OkClassName}.delete";

    public async Task<bool> DeleteAsync(string productId, IRequestContext context, CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertProductId(productId);
        
        switch (context)
        {
            case MainAccountRequestContext: 
            case ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            DeleteMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }

    public AnchorNavigator<TDto> GetByCatalogNavigator<TDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration, IEnumerable<string>? fields = null, CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        return new AnchorNavigator<TDto>(configuration => GetByCatalogAsync<TDto>(context, configuration, fields, cancellationToken), anchorConfiguration);
    }
    
    private const string GetCatalogsByGroupMethodName = $"{OkClassName}.getByCatalog";
    
    private async Task<AnchorResponse<TDto>> GetByCatalogAsync<TDto>(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        fields ??= [ShortProductBeanFields.Id];
        
        var parameters = new RestParameters()
            .InsertAnchor(anchorConfiguration.Anchor)
            .InsertDirection(anchorConfiguration.Direction)
            .InsertCount(anchorConfiguration.Count)
            .InsertFields(fields.ToArray());
        
        switch (context)
        {
            case GroupCatalogsRequestContext:
            case GroupRequestContext:
            case MainGroupRequestContext: 
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext),
                    nameof(GroupCatalogsRequestContext));
        }
        
        var response = await okApi.CallAsync<ShortProductsResponse<TDto>>(
            GetCatalogsByGroupMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<TDto>()
        {
            Anchor = response.Anchor,
            HasMore = response.HasMore,
            TotalCount = response.TotalCount,
            Results = response.Products
        };
    }

    public AnchorNavigator<TDto> GetProductsNavigator<TDto>(IRequestContext context, AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null, bool onModeration = false, CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        return new AnchorNavigator<TDto>(configuration => GetProductsAsync<TDto>(context, configuration, fields, onModeration, cancellationToken), anchorConfiguration);
    }
    
    private const string GetProductsMethodName = $"{OkClassName}.getProducts";

    private async Task<AnchorResponse<TDto>> GetProductsAsync<TDto>(
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        bool onModeration = false,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        fields ??= [ShortProductBeanFields.Id];
        
        var parameters = new RestParameters()
            .InsertAnchor(anchorConfiguration.Anchor)
            .InsertDirection(anchorConfiguration.Direction)
            .InsertCount(anchorConfiguration.Count)
            .InsertFields(fields.ToArray());
        
        switch (context)
        {
            case GroupRequestContext:
            case MainGroupRequestContext:
                parameters = context.Apply(parameters)
                    .InsertTab(onModeration ? ApiProductTab.OnModeration : ApiProductTab.Products);
                break;
            case MainAccountRequestContext: 
            case ExplicitTokenRequestContext:
                parameters = context.Apply(parameters)
                    .InsertTab(ApiProductTab.Own);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<ShortProductsResponse<TDto>>(
            GetProductsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return new AnchorResponse<TDto>()
        {
            Anchor = response.Anchor,
            HasMore = response.HasMore,
            TotalCount = response.TotalCount,
            Results = response.Products
        };
    }

    private const string GetByIdsMethodName = $"{OkClassName}.getByIds";

    public async Task<ICollection<TDto>> GetByIdsAsync<TDto>(ICollection<string> productIds, IRequestContext context, IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        if (productIds.Count == 0)
        {
            return Array.Empty<TDto>();
        }
        
        fields ??= [MediaTopicBeanFields.MediaText, MediaTopicBeanFields.Id];
        
        var parameters = new RestParameters()
            .InsertProductIds(productIds.ToArray())
            .InsertFields(fields.ToArray());
        
        switch (context)
        {
            case MainAccountRequestContext:
            case ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }
        
        var response = await okApi.CallAsync<ProductsResponse<TDto>>(
            GetByIdsMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response.Products;
    }

    private const string PinMethodName = $"{OkClassName}.pin";

    public async Task<bool> PinAsync(string productId,
        bool on,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertProductId(productId)
            .InsertCustomParameter("on", on);
        
        switch (context)
        {
            //Добавить контекст каталог без указания группы
            case MainAccountRequestContext:
            case ExplicitTokenRequestContext:
            case GroupCatalogsRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext),
                    nameof(GroupCatalogsRequestContext));
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            PinMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response.Success;
    }

    private const string ReorderMethodName = $"{OkClassName}.reorder";

    public async Task<bool> ReorderAsync(string productId, IRequestContext context, string? afterProductId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertProductId(productId)
            .InsertCustomParameter("after_product_id", afterProductId);
        
        switch (context)
        {
            case GroupCatalogsRequestContext:
            case GroupRequestContext: 
            case MainGroupRequestContext: 
            case MainAccountRequestContext:
            case ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(GroupCatalogsRequestContext),
                    nameof(GroupRequestContext),
                    nameof(MainGroupRequestContext),
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext)
                    );
        }
        
        var response = await okApi.CallAsync<CompletionStatusResponse>(
            ReorderMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        return response?.Success ?? false;
    }
}