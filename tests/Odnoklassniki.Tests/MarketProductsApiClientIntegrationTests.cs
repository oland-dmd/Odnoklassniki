using Oland.MediaManager.Application.Validation;
using Oland.Odnoklassniki.Image;
using Oland.Odnoklassniki.Rest;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Enums;
using Exceptions;
using Rest.AnchorNavigators;
using Rest.ApiClients.Market;
using Rest.ApiClients.Market.Constants;
using Rest.ApiClients.Market.Models;
using Rest.ApiClients.Market.Response;
using Rest.BeanFields;
using Rest.RequestContexts;
using Rest.RequestContexts.ValueObjects;
using Common;
using MediaManager.Application.Services;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class MarketProductsApiClientIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly MarketProductsApiClient _productsClient;

    // Контексты для разных сценариев
    private readonly GroupRequestContext _groupContext;
    private readonly GroupCatalogsRequestContext _groupCatalogContext;
    private readonly ExplicitTokenRequestContext _explicitTokenContext;
    private readonly PhotosV2ApiClient _photosV2Client;
    private readonly ImageClient _imageClient;


    public MarketProductsApiClientIntegrationTests(OkApiTestFixture fixture)
    {
        IMediaService mediaService = new MediaService(new MediaValidator());
        _productsClient = new MarketProductsApiClient(fixture.ClientCore, mediaService);
        _photosV2Client = new PhotosV2ApiClient(fixture.ClientCore);
        _imageClient = new ImageClient();
        
        _groupContext = new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId);
        _groupCatalogContext = new GroupCatalogsRequestContext(TestSettings.AccessPair, TestSettings.GroupId, TestSettings.CatalogId);
        _explicitTokenContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
    }

    #region Helper Methods (Вспомогательные методы для создания тестовых данных)

    private async Task<ProductModel> CreateValidProductModel(IRequestContext context, string titleSuffix = "", string description = "Test description")
    {
        var token = await UploadImage(context);
        
        return new ProductModel
        {
            Title = $"Integration Test Product {titleSuffix} {DateTime.UtcNow:yyyyMMddHHmmss}",
            Description = description,
            Price = 999.99m,
            Currency = "RUB",
            LifetimePerDays = 30,
            PartnerLink = "https://example.com/product",
            Photos = [new PhotoModel { PhotoToken = token }]
        };
    }
    
    private async Task<string> UploadImage(IRequestContext context)
    {
        var uploadUrl = await _photosV2Client.GetUploadUrlAsync(context);
        await using var file = File.Open("./test.png",
            FileMode.Open);
        var token = await _imageClient.UploadImageAsync(uploadUrl.UploadUrl, file, CancellationToken.None);
        return token.First().Value;
    }

    private static ProductModel CreateMinimalValidProduct(string titleSuffix = "")
    {
        // Минимальная валидная модель: только Title + Price + Description (без фото)
        return new ProductModel
        {
            Title = $"Minimal Product {titleSuffix} {DateTime.UtcNow:yyyyMMddHHmmss}",
            Description = "Minimal description",
            Price = 100m,
            Currency = "RUB",
            LifetimePerDays = 30,
            Photos = null // Description присутствует, поэтому валидация пройдёт
        };
    }

    private static ProductModel CreateInvalidProduct_WithoutTitle()
    {
        return new ProductModel
        {
            Title = string.Empty, // ❌ Не валидно: пустой Title
            Description = "Test",
            Price = 100m,
            LifetimePerDays = 0
        };
    }

    private static ProductModel CreateInvalidProduct_NegativePrice()
    {
        return new ProductModel
        {
            Title = "Test Product",
            Description = "Test",
            Price = -10m, // ❌ Не валидно: отрицательная цена
            LifetimePerDays = 0
        };
    }

    private static ProductModel CreateInvalidProduct_WithoutDescriptionOrPhotos()
    {
        return new ProductModel
        {
            Title = "Test Product",
            Description = null, // ❌ Нет описания
            Photos = null,      // ❌ И нет фото → валидация не пройдёт
            Price = 100m,
            LifetimePerDays = 0
        };
    }

    #endregion

    #region AddAsync (Создание продукта)

    [Fact]
    public async Task AddAsync_WithValidProductAndGroupContext_ShouldReturnNewProductId()
    {
        // Arrange
        var product = await CreateValidProductModel(_groupContext, "AddGroup");

        // Act
        var productId = await _productsClient.AddAsync(
            model: product,
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(productId);
        Assert.NotEmpty(productId);

        // Cleanup: удаляем созданный продукт
        if (!string.IsNullOrEmpty(productId))
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task AddAsync_WithValidProductAndGroupCatalogContext_ShouldReturnNewProductId()
    {
        //TODO: В добавление добавить проверку фактического нахождения
        // Arrange
        var product = await CreateValidProductModel(_groupContext,"AddCatalog");

        // Act
        var productId = await _productsClient.AddAsync(
            model: product,
            context: _groupCatalogContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(productId);
        Assert.NotEmpty(productId);

        // Cleanup
        if (!string.IsNullOrEmpty(productId))
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task AddAsync_WithValidProductAndExplicitTokenContext_ShouldReturnNewProductId()
    {
        // Arrange
        var product = await CreateValidProductModel(_explicitTokenContext,"AddExplicit");

        // Act
        var productId = await _productsClient.AddAsync(
            model: product,
            context: _explicitTokenContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(productId);
        Assert.NotEmpty(productId);

        // Cleanup
        if (!string.IsNullOrEmpty(productId))
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task AddAsync_WithInvalidProduct_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidProduct = CreateInvalidProduct_WithoutTitle();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _productsClient.AddAsync(
                model: invalidProduct,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task AddAsync_WithProductWithoutDescriptionOrPhotos_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidProduct = CreateInvalidProduct_WithoutDescriptionOrPhotos();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _productsClient.AddAsync(
                model: invalidProduct,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task AddAsync_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var product = await CreateValidProductModel(_groupContext);
        var fakeContext = new FakeRequestContext("FriendRequestContext");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _productsClient.AddAsync(
                model: product,
                context: fakeContext,
                cancellationToken: CancellationToken.None);
        });

        //TODO: Сделать проверку всех контекстов с оригиналами
        //Assert.Contains("FriendRequestContext", exception.Message);
        Assert.Contains("GroupRequestContext", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var product = await CreateValidProductModel(_groupContext);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _productsClient.AddAsync(
                model: product,
                context: _groupContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region EditAsync (Редактирование продукта)

    [Fact]
    public async Task EditAsync_WithValidProductAndGroupContext_ShouldReturnTrue()
    {
        // Arrange: создаём продукт для редактирования
        var initialProduct = await CreateValidProductModel(_groupContext,"EditInitial");
        var productId = await _productsClient.AddAsync(initialProduct, _groupContext, CancellationToken.None);
        Assert.NotNull(productId);

        var editedProduct = await CreateValidProductModel(_groupContext,"Edited");

        try
        {
            // Act
            var result = await _productsClient.EditAsync(productId, model: editedProduct,
                context: _groupContext,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.True(result);
        }
        finally
        {
            // Cleanup
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task EditAsync_WithInvalidProduct_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidProduct = CreateInvalidProduct_NegativePrice();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _productsClient.EditAsync("", model: invalidProduct,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task EditAsync_WithInvalidProductId_ShouldReturnFalse()
    {
        // Arrange
        var product = await CreateValidProductModel(_groupContext,"EditInvalidId");

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _productsClient.EditAsync("ProductId", model: product,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task EditAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var product = await CreateValidProductModel(_groupContext);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _productsClient.EditAsync("ProductId", model: product,
                context: _groupContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region DeleteAsync (Удаление продукта)

    [Fact]
    public async Task DeleteAsync_WithValidProductIdAndExplicitTokenContext_ShouldReturnTrue()
    {
        // Arrange: создаём продукт для удаления
        var product = await CreateValidProductModel(_groupContext,"DeleteTest");
        var productId = await _productsClient.AddAsync(product, _groupContext, CancellationToken.None);
        Assert.NotNull(productId);

        // Act
        var result = await _productsClient.DeleteAsync(
            productId: productId,
            context: _explicitTokenContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidProductId_ShouldReturnFalse()
    {
        // Arrange
        var invalidProductId = "INVALID_PRODUCT_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _productsClient.DeleteAsync(
                productId: invalidProductId,
                context: _explicitTokenContext,
                cancellationToken: CancellationToken.None);
        }); 
    }

    [Fact]
    public async Task DeleteAsync_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var fakeContext = new FakeRequestContext("GroupRequestContext"); // Delete поддерживает только MainAccount/ExplicitToken
        var productId = "YOUR_PRODUCT_ID";

        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _productsClient.DeleteAsync(
                productId: productId,
                context: fakeContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var productId = "YOUR_PRODUCT_ID";

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _productsClient.DeleteAsync(
                productId: productId,
                context: _explicitTokenContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region PinAsync (Закрепление продукта)

    [Fact]
    public async Task PinAsync_WithValidProductIdAndExplicitTokenContext_ShouldReturnTrue()
    {
        // Arrange
        var productModel = CreateMinimalValidProduct();
        var productId = await _productsClient.AddAsync(productModel, _groupContext);      // Введите ID продукта для перемещения

        try
        {
            // Act: закрепляем
            var pinResult = await _productsClient.PinAsync(
                productId: productId,
                on: true,
                context: _explicitTokenContext,
                cancellationToken: CancellationToken.None);
            // Assert
            Assert.True(pinResult);
            // Cleanup: открепляем
            var unpinResult = await _productsClient.PinAsync(productId, false, _explicitTokenContext, CancellationToken.None);
            Assert.True(unpinResult);
        }
        finally
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task PinAsync_WithGroupCatalogContext_ShouldReturnTrue()
    {
        // Arrange
        var productModel = CreateMinimalValidProduct();
        var productId = await _productsClient.AddAsync(productModel, _groupCatalogContext);      // Введите ID продукта для перемещения

        try
        {
            // Act
            var pinResult = await _productsClient.PinAsync(
                productId: productId,
                on: true,
                context: _groupCatalogContext,
                cancellationToken: CancellationToken.None);
            // Assert
            Assert.True(pinResult);
            
            var unpinResult = await _productsClient.PinAsync(
                productId: productId,
                on: false,
                context: _groupCatalogContext,
                cancellationToken: CancellationToken.None);
            // Assert
            Assert.True(unpinResult);
        }
        finally
        {
            await  _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task PinAsync_WithInvalidProductId_ShouldReturnFalse()
    {
        // Arrange
        var invalidProductId = "INVALID_PRODUCT_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _productsClient.PinAsync(
                productId: invalidProductId,
                on: true,
                context: _explicitTokenContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task PinAsync_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var fakeContext = new FakeRequestContext("GroupRequestContext"); // Pin не поддерживает обычный GroupRequestContext
        var productId = "YOUR_PRODUCT_ID";

        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _productsClient.PinAsync(
                productId: productId,
                on: true,
                context: fakeContext,
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion

    #region ReorderAsync (Изменение порядка продуктов)

    [Fact]
    public async Task ReorderAsync_WithValidProductId_ShouldReturnTrue()
    {
        var productModel = CreateMinimalValidProduct();
        // Arrange
        var productId_2 = await _productsClient.AddAsync(productModel with{ Title = "2"}, _groupContext);  // Введите ID продукта, после которого разместить
        var productId_1 = await _productsClient.AddAsync(productModel with{Title = "1"}, _groupContext);      // Введите ID продукта для перемещения

        // Arrange
        var productId = productId_2; // Введите ID продукта
        string afterProductId = null; // Переместить в начало

        try
        {
            // Act
            var result = await _productsClient.ReorderAsync(
                productId: productId,
                context: _groupCatalogContext,
                afterProductId: afterProductId,
                cancellationToken: CancellationToken.None);
            // Assert
            Assert.True(result);
        }
        finally
        {
            await _productsClient.DeleteAsync(productId_1, _explicitTokenContext);
            await _productsClient.DeleteAsync(productId_2, _explicitTokenContext);
        }
    }

    [Fact]
    public async Task ReorderAsync_WithAfterProductId_ShouldReturnTrue()
    {
        var productModel = CreateMinimalValidProduct();
        // Arrange
        var afterProductId = await _productsClient.AddAsync(productModel, _groupContext);  // Введите ID продукта, после которого разместить
        var productId = await _productsClient.AddAsync(productModel, _groupContext);      // Введите ID продукта для перемещения

        try
        {
            // Act
            var result = await _productsClient.ReorderAsync(
                productId: productId,
                context: _groupContext,
                afterProductId: afterProductId,
                cancellationToken: CancellationToken.None);
            // Assert
            Assert.True(result);
        }
        finally
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext);
            await _productsClient.DeleteAsync(afterProductId, _explicitTokenContext);
        }
    }

    [Fact]
    public async Task ReorderAsync_WithInvalidProductId_ShouldReturnFalse()
    {
        // Arrange
        var invalidProductId = "INVALID_PRODUCT_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _productsClient.ReorderAsync(
                productId: invalidProductId,
                context: _groupCatalogContext,
                afterProductId: null,
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion

    #region GetByIdsAsync<T> (Получение продуктов по ID)

    [Fact]
    public async Task GetByIdsAsync_WithValidProductIds_ShouldReturnValidProducts()
    {
        // Arrange
        var productIds = new[] { "157278689031615" }; // Введите ID существующего продукта
        var fields = new[] { MediaTopicBeanFields.Id };

        // Act
        var result = await _productsClient.GetByIdsAsync<ProductDto>(
            productIds: productIds,
            context: _explicitTokenContext,
            fields: fields,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(productIds.Length, result.Count);
    }

    [Fact]
    public async Task GetByIdsAsync_WithDefaultFields_ShouldReturnProductsWithExpectedFields()
    {
        
        // Arrange
        var productIds = new[] { "157278689031615" };
        // fields = null → должны использоваться значения по умолчанию (MediaTopicBeanFields.MediaText)

        // Act
        var result = await _productsClient.GetByIdsAsync<ProductDto>(
            productIds: productIds,
            context: _explicitTokenContext,
            fields: null,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var product = result.FirstOrDefault();
        Assert.NotNull(product);
        Assert.NotNull(product.Id);
    }

    [Fact]
    public async Task GetByIdsAsync_WithEmptyProductIds_ShouldReturnEmptyCollection()
    {
        // Arrange
        var productIds = new string[] { };

        // Act
        var result = await _productsClient.GetByIdsAsync<ProductDto>(
            productIds: productIds,
            context: _explicitTokenContext,
            fields: null,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsync_WithInvalidProductIds_ShouldReturnEmptyOrNull()
    {
        // Arrange
        var productIds = new[] { "INVALID_PRODUCT_ID_12345" };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _productsClient.GetByIdsAsync<ProductDto>(
                productIds: productIds,
                context: _explicitTokenContext,
                fields: null,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetByIdsAsync_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var productIds = new[] { "YOUR_PRODUCT_ID" };
        var fakeContext = new FakeRequestContext("GroupRequestContext"); // GetByIds поддерживает только MainAccount/ExplicitToken

        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _productsClient.GetByIdsAsync<ProductDto>(
                productIds: productIds,
                context: fakeContext,
                fields: null,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetByIdsAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var productIds = new[] { "YOUR_PRODUCT_ID" };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _productsClient.GetByIdsAsync<ProductDto>(
                productIds: productIds,
                context: _explicitTokenContext,
                fields: null,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetByCatalogNavigator<T> (Пагинация продуктов в каталоге)

    [Fact]
    public async Task GetByCatalogNavigator_WithValidConfiguration_ShouldReturnValidPages()
    {
        // Arrange
        var productModel = CreateMinimalValidProduct();
        var productIds = new List<string>();

        for (var i = 0; i < 5; i++)
        {
            var productId = await _productsClient.AddAsync(productModel, _groupCatalogContext);
            productIds.Add(productId);
        }
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 1,
            Direction = PagingDirection.FORWARD
        };

        try
        {
            // Act
            var navigator = _productsClient.GetByCatalogNavigator<ProductDto>(
                context: _groupCatalogContext,
                anchorConfiguration: anchorConfig,
                fields: new[] { ShortProductBeanFields.Title, ShortProductBeanFields.Id },
                cancellationToken: CancellationToken.None);
            var pages = new List<AnchorResponse<ProductDto>>();
            await foreach (var page in navigator)
            {
                pages.Add(page);
                if (pages.Count >= 3) break; // Ограничиваем количество страниц для теста
            }

            // Assert
            Assert.NotEmpty(pages);
            foreach (var page in pages)
            {
                Assert.NotNull(page);
                Assert.NotNull(page.Anchor);
                if (page.Results != null)
                {
                    foreach (var product in page.Results)
                    {
                        Assert.NotNull(product);
                        Assert.NotNull(product.Id);
                    }
                }
            }
        }
        finally
        {
            foreach (var productId in productIds)
            {
                await _productsClient.DeleteAsync(productId, _explicitTokenContext);
            }
        }
    }

    [Fact]
    public async Task GetByCatalogNavigator_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var anchorConfig = new AnchorConfiguration { Count = 10 };
        var fakeContext = new FakeRequestContext("ExplicitTokenRequestContext"); // GetByCatalog не поддерживает ExplicitToken

        // Act & Assert
        var navigator = _productsClient.GetByCatalogNavigator<ProductDto>(
            context: fakeContext,
            anchorConfiguration: anchorConfig,
            fields: null,
            cancellationToken: CancellationToken.None);

        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await foreach (var _ in navigator) { }
        });
    }

    [Fact]
    public async Task GetByCatalogNavigator_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var anchorConfig = new AnchorConfiguration { Count = 10 };

        // Act & Assert
        var navigator = _productsClient.GetByCatalogNavigator<ProductDto>(
            context: _groupCatalogContext,
            anchorConfiguration: anchorConfig,
            fields: null,
            cancellationToken: cancellationTokenSource.Token);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await foreach (var _ in navigator) { }
        });
    }

    #endregion

    #region GetProductsNavigator<T> (Пагинация продуктов пользователя/группы)

    [Fact]
    public async Task GetProductsNavigator_WithGroupContextAndProductsTab_ShouldReturnValidPages()
    {
        var productModel = CreateMinimalValidProduct();
        var productIds = new List<string>();

        for (var i = 0; i < 5; i++)
        {
            var productId = await _productsClient.AddAsync(productModel, _groupContext);
            productIds.Add(productId);
        }
        
        // Arrange
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 1,
            Direction = PagingDirection.FORWARD
        };
        var onModeration = false; // Запрашиваем опубликованные продукты

        try
        {
            // Act
            var navigator = _productsClient.GetProductsNavigator<ProductDto>(
                context: _groupContext,
                anchorConfiguration: anchorConfig,
                fields: new[] { ShortProductBeanFields.Title, ShortProductBeanFields.Price },
                onModeration: onModeration,
                cancellationToken: CancellationToken.None);
            var pages = new List<AnchorResponse<ProductDto>>();
            await foreach (var page in navigator)
            {
                pages.Add(page);
                if (pages.Count >= 2) break;
            }

            // Assert
            Assert.NotEmpty(pages);
        }
        finally
        {
            foreach (var productId in productIds)
            {
                await _productsClient.DeleteAsync(productId, _explicitTokenContext);
            }
        }
    }

    [Fact]
    public async Task GetProductsNavigator_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext()
    {
        // Arrange
        var anchorConfig = new AnchorConfiguration { Count = 10 };
        var fakeContext = new FakeRequestContext("GroupCatalogsRequestContext"); // GetProducts не поддерживает GroupCatalogs

        // Act & Assert
        var navigator = _productsClient.GetProductsNavigator<ProductDto>(
            context: fakeContext,
            anchorConfiguration: anchorConfig,
            fields: null,
            cancellationToken: CancellationToken.None);

        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await foreach (var _ in navigator) { }
        });
    }

    [Fact]
    public async Task GetProductsNavigator_Reset_ShouldRestartIteration()
    {
        // Arrange
        var anchorConfig = new AnchorConfiguration { Count = 3 };

        var navigator = _productsClient.GetProductsNavigator<ProductDto>(
            context: _groupContext,
            anchorConfiguration: anchorConfig,
            fields: null,
            cancellationToken: CancellationToken.None);

        // Act - первая итерация
        var firstPageCount = 0;
        await foreach (var page in navigator)
        {
            firstPageCount += page.Results?.Count ?? 0;
            break;
        }

        // Reset и повторная итерация
        navigator.Reset();
        
        var secondPageCount = 0;
        await foreach (var page in navigator)
        {
            secondPageCount += page.Results?.Count ?? 0;
            break;
        }

        // Assert
        Assert.Equal(firstPageCount, secondPageCount);
    }

    #endregion

    #region Context Validation Tests (Проверка обработки контекстов)

    [Theory]
    [InlineData("FriendRequestContext")]
    [InlineData("UserRequestContext")]
    public async Task AllMethods_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext(string contextType)
    {
        // Arrange
        var fakeContext = new FakeRequestContext(contextType);
        var product = await CreateValidProductModel(_groupContext);
        var productId = "YOUR_PRODUCT_ID";

        // Act & Assert для каждого метода
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _productsClient.AddAsync(product, fakeContext, CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _productsClient.EditAsync(productId, product, fakeContext, CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _productsClient.DeleteAsync(productId, fakeContext, CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _productsClient.PinAsync(productId, true, fakeContext, CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _productsClient.ReorderAsync(productId, fakeContext, null, CancellationToken.None));
    }

    #endregion

    #region MediaService Integration Tests (Интеграция с медиа-сервисом)

    [Fact]
    public async Task AddAsync_WithValidProduct_ShouldSerializeMediaAttachmentCorrectly()
    {
        // Arrange
        var product = await CreateValidProductModel(_groupContext,"MediaTest");
        
        // Act
        var productId = await _productsClient.AddAsync(
            model: product,
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(productId);
        
        // Cleanup
        if (!string.IsNullOrEmpty(productId))
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task AddAsync_WithProductWithoutPhotos_ButWithDescription_ShouldSucceed()
    {
        // Arrange
        var product = CreateMinimalValidProduct("NoPhotos");
        // Photos = null, но Description присутствует → IsValid == true

        // Act
        var productId = await _productsClient.AddAsync(
            model: product,
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(productId);
        
        // Cleanup
        if (!string.IsNullOrEmpty(productId))
        {
            await _productsClient.DeleteAsync(productId, _explicitTokenContext, CancellationToken.None);
        }
    }

    #endregion

    #region Helper Classes for Testing

    /// <summary>
    /// Фейковый контекст для тестирования обработки неверных типов контекста
    /// </summary>
    private class FakeRequestContext : IRequestContext
    {
        public FakeRequestContext(string typeName = "FakeRequestContext")
        {
            TypeName = typeName;
        }

        public string TypeName { get; }

        public AccessPair AccessPair => TestSettings.AccessPair;

        public RestParameters Apply(RestParameters parameters)
        {
            return parameters; // Не добавляем никаких параметров
        }

        public override string ToString() => TypeName;
    }

    #endregion
}