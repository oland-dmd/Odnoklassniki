using Oland.Odnoklassniki.Image;
using Oland.Odnoklassniki.Rest;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;
using Oland.Odnoklassniki.Rest.BeanFields;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Enums;
using Exceptions;
using Rest.AnchorNavigators;
using Rest.ApiClients.Market;
using Rest.RequestContexts;
using Rest.RequestContexts.ValueObjects;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class MarketCatalogsApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly MarketCatalogsApiClient _marketClient = new(fixture.ClientCore);
    private readonly GroupRequestContext _groupContext = new(TestSettings.AccessPair,
        TestSettings.GroupId);
    private readonly PhotosV2ApiClient _photosV2Client = new(fixture.ClientCore);
    private readonly ImageClient _imageClient = new();

    // Создаём валидный контекст для группы с реальными токенами

    #region AddAsync (Создание каталога)

    [Fact]
    public async Task AddAsync_WithValidData_ShouldReturnNewCatalogId()
    {
        var token = await UploadImage();

        // Arrange
        var title = $"Integration Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var photoId = token; // Введите ID фотографии для обложки
        var adminRestricted = false;

        // Act
        var catalogId = await _marketClient.AddAsync(
            title: title,
            photoId: photoId,
            adminRestricted: adminRestricted,
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(catalogId);
        Assert.NotEmpty(catalogId);

        // Cleanup: удаляем созданный каталог
        if (!string.IsNullOrEmpty(catalogId))
        {
            await _marketClient.DeleteAsync(catalogId, deleteProducts: false, _groupContext, CancellationToken.None);
        }
    }

    private async Task<string> UploadImage()
    {
        var uploadUrl = await _photosV2Client.GetUploadUrlAsync(_groupContext);
        await using var file = File.Open("./test.png",
            FileMode.Open);
        var token = await _imageClient.UploadImageAsync(uploadUrl.UploadUrl, file, CancellationToken.None);
        return token.First().Value;
    }

    [Fact]
    public async Task AddAsync_WithEmptyTitle_ShouldThrowException()
    {
        var token =  await UploadImage();
        // Arrange
        var emptyTitle = string.Empty;
        var photoId = token;
        var adminRestricted = false;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _marketClient.AddAsync(
                title: emptyTitle,
                photoId: photoId,
                adminRestricted: adminRestricted,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task AddAsync_WithInvalidGroupId_ShouldThrowException()
    {
        var token = await UploadImage();
        // Arrange
        var invalidContext = new GroupRequestContext(TestSettings.AccessPair,
            new GroupId("INVALID_GROUP_ID_12345"));
        
        var title = $"Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var photoId = token;
        var adminRestricted = false;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _marketClient.AddAsync(
                title: title,
                photoId: photoId,
                adminRestricted: adminRestricted,
                context: invalidContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task AddAsync_WithInvalidRequestContext_ShouldThrowUnexpectedRequestContext()
    {
        var token = await UploadImage();
        // Arrange
        var fakeContext = new FakeRequestContext(); // Неверный тип контекста
        var title = $"Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var photoId = token;
        var adminRestricted = false;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _marketClient.AddAsync(
                title: title,
                photoId: photoId,
                adminRestricted: adminRestricted,
                context: fakeContext,
                cancellationToken: CancellationToken.None);
        });

        // Проверяем сообщение исключения
        Assert.Contains("FakeRequestContext", exception.Message);
        Assert.Contains("GroupRequestContext", exception.Message);
        Assert.Contains("MainGroupRequestContext", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        
        var title = $"Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var photoId = "YOUR_PHOTO_ID";
        var adminRestricted = false;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _marketClient.AddAsync(
                title: title,
                photoId: photoId,
                adminRestricted: adminRestricted,
                context: _groupContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region EditAsync (Редактирование каталога)

    [Fact]
    public async Task EditAsync_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        // Сначала создаём каталог для редактирования
        var initialTitle = $"Edit Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var adminRestricted = false;
        
        var catalogId = await _marketClient.AddAsync(
            initialTitle, adminRestricted, _groupContext, cancellationToken: CancellationToken.None);
        
        Assert.NotNull(catalogId);

        var token = await UploadImage();
        
        var newTitle = $"Edited Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var newPhotoId = token; // Введите ID новой фотографии

        try
        {
            // Act
            var result = await _marketClient.EditAsync(
                catalogId: catalogId,
                title: newTitle,
                adminRestricted: adminRestricted,
                context: _groupContext,
                photoId: newPhotoId, cancellationToken: CancellationToken.None);

            // Assert
            Assert.True(result);
        }
        finally
        {
            // Cleanup
            await _marketClient.DeleteAsync(catalogId, false, _groupContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task EditAsync_WithInvalidCatalogId_ShouldReturnFalse()
    {
        // Arrange
        var invalidCatalogId = "INVALID_CATALOG_ID_12345";
        var title = $"Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var adminRestricted = false;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _marketClient.EditAsync(
                catalogId: invalidCatalogId,
                title: title,
                adminRestricted: adminRestricted,
                context: _groupContext, photoId: null,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task EditAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        
        var catalogId = "YOUR_CATALOG_ID"; // Введите ID существующего каталога
        var title = $"Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var photoId = "YOUR_PHOTO_ID";
        var adminRestricted = false;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _marketClient.EditAsync(
                catalogId: catalogId,
                title: title,
                adminRestricted: adminRestricted,
                context: _groupContext,
                photoId: photoId, cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region DeleteAsync (Удаление каталога)

    [Fact]
    public async Task DeleteAsync_WithValidCatalogId_ShouldReturnTrue()
    {
        // Arrange
        // Создаём временный каталог для удаления
        var title = $"Delete Test Catalog {DateTime.UtcNow:yyyyMMddHHmmss}";
        var adminRestricted = false;
        
        var catalogId = await _marketClient.AddAsync(
            title, adminRestricted, _groupContext,  cancellationToken: CancellationToken.None);
        
        Assert.NotNull(catalogId);

        // Act
        var result = await _marketClient.DeleteAsync(
            catalogId: catalogId,
            deleteProducts: false,
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidCatalogId_ShouldReturnFalse()
    {
        // Arrange
        var invalidCatalogId = "INVALID_CATALOG_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _marketClient.DeleteAsync(
                catalogId: invalidCatalogId,
                deleteProducts: false,
                context: _groupContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteAsync_WithDeleteProductsTrue_ShouldReturnTrue()
    {
        // Arrange
        var title = $"Delete With Products Test {DateTime.UtcNow:yyyyMMddHHmmss}";
        var adminRestricted = false;
        
        var catalogId = await _marketClient.AddAsync(
            title, adminRestricted, _groupContext);
        
        Assert.NotNull(catalogId);

        // Act
        var result = await _marketClient.DeleteAsync(
            catalogId: catalogId,
            deleteProducts: true, // Удаляем вместе с товарами
            context: _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ReorderAsync (Изменение порядка каталогов)

    [Fact]
    public async Task ReorderAsync_WithValidCatalogId_ShouldReturnTrue()
    {
        var second = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var first = await _marketClient.AddAsync(
            $"first {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);

        // Arrange
        string afterCatalogId = null; // Переместить в начало
        try
        {
            // Act
            var result = await _marketClient.ReorderAsync(
                catalogId: second,
                context: _groupContext,
                afterCatalogId: afterCatalogId,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.True(result);
        }
        finally
        {
            await _marketClient.DeleteAsync(first, false, _groupContext);
            await _marketClient.DeleteAsync(second, false, _groupContext);
        }
    }

    [Fact]
    public async Task ReorderAsync_WithInvalidCatalogId_ShouldReturnFalse()
    {
        // Arrange
        var invalidCatalogId = "INVALID_CATALOG_ID_12345";
        string afterCatalogId = null;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _marketClient.ReorderAsync(
                catalogId: invalidCatalogId,
                context: _groupContext,
                afterCatalogId: afterCatalogId,
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion

    #region GetByIdsAsync<T> (Получение каталогов по ID)

    [Fact]
    public async Task GetByIdsAsync_WithValidCatalogIds_ShouldReturnValidCatalogs()
    {
        var second = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var first = await _marketClient.AddAsync(
            $"first {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);

        // Arrange
        var catalogIds = new[] { first, second }; // Введите ID существующего каталога
        var fields = new[] { CatalogBeanFields.UserId, CatalogBeanFields.Name, CatalogBeanFields.Capabilities };
        try
        {
            // Act
            var result = await _marketClient.GetByIdsAsync<CatalogDto>(
                catalogIds: catalogIds,
                context: _groupContext,
                fields: fields,
                cancellationToken: CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(catalogIds.Length, result.Count);
        }
        finally
        {
            await _marketClient.DeleteAsync(first, false, _groupContext);
            await _marketClient.DeleteAsync(second, false, _groupContext);
        }
    }

    [Fact]
    public async Task GetByIdsAsync_WithEmptyCatalogIds_ShouldReturnEmptyCollection()
    {
        // Arrange
        var catalogIds = new string[] { };
        var fields = new[] { CatalogBeanFields.Name };

        // Act
        var result = await _marketClient.GetByIdsAsync<CatalogDto>(
            catalogIds: catalogIds,
            context: _groupContext,
            fields: fields,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsync_WithInvalidCatalogIds_ShouldReturnEmptyOrNull()
    {
        // Arrange
        var catalogIds = new[] { "INVALID_CATALOG_ID_12345" };
        var fields = new[] { CatalogBeanFields.Name };

        //Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _marketClient.GetByIdsAsync<CatalogDto>(
                catalogIds: catalogIds,
                context: _groupContext,
                fields: fields,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetByIdsAsync_WithDefaultFields_ShouldReturnCatalogsWithExpectedFields()
    {
        var catalogId = await _marketClient.AddAsync(
            $"first {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        // Arrange
        var catalogIds = new[] { catalogId }; // Введите ID каталога
        // fields = null → должны использоваться значения по умолчанию

        try
        {
            // Act
            var result = await _marketClient.GetByIdsAsync<CatalogDto>(
                catalogIds: catalogIds,
                context: _groupContext,
                fields: null, // Используем поля по умолчанию
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var catalog = result.FirstOrDefault();
            Assert.NotNull(catalog);
            Assert.NotNull(catalog.Id);
        }
        finally
        {
            await _marketClient.DeleteAsync(catalogId, false, _groupContext);
        }
    }

    [Fact]
    public async Task GetByIdsAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var catalogIds = new[] { "YOUR_CATALOG_ID" };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _marketClient.GetByIdsAsync<CatalogDto>(
                catalogIds: catalogIds,
                context: _groupContext,
                fields: null,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetByGroupAnchorNavigator<T> (Пагинация каталогов)

    [Fact]
    public async Task GetByGroupAnchorNavigator_WithValidConfiguration_ShouldReturnValidPages()
    {
        var first = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var second = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var third = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var fourth = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        // Arrange
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 1,
            Direction = PagingDirection.FORWARD
        };

        try
        {
            // Act
            var navigator = _marketClient.GetByGroupAnchorNavigator<CatalogDto>(
                context: _groupContext,
                anchorConfiguration: anchorConfig,
                fields: [CatalogBeanFields.Name],
                cancellationToken: CancellationToken.None);

            var pages = new List<AnchorResponse<CatalogDto>>();
            await foreach (var page in navigator)
            {
                pages.Add(page);
                if (pages.Count >= 4) break; // Ограничиваем количество страниц для теста
            }

            // Assert
            Assert.NotEmpty(pages);
            foreach (var page in pages)
            {
                Assert.NotNull(page);
                Assert.NotNull(page.Anchor);
                // Results может быть null или пустым на последней странице
                if (page.Results == null) continue;
                
                foreach (var catalog in page.Results)
                {
                    Assert.NotNull(catalog);
                    Assert.NotNull(catalog.Id);
                }
            }
        }
        finally
        {
            await _marketClient.DeleteAsync(first, false, _groupContext);
            await _marketClient.DeleteAsync(second, false, _groupContext);
            await _marketClient.DeleteAsync(third, false, _groupContext);
            await _marketClient.DeleteAsync(fourth, false, _groupContext);
        }
    }

    [Fact]
    public async Task GetByGroupAnchorNavigator_WithCustomFields_ShouldReturnCatalogsWithRequestedFields()
    {
        // Arrange
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 5,
            Direction = PagingDirection.FORWARD
        };
        var fields = new[] { CatalogBeanFields.Name }; // Запрашиваем только имя

        // Act
        var navigator = _marketClient.GetByGroupAnchorNavigator<CatalogDto>(
            context: _groupContext,
            anchorConfiguration: anchorConfig,
            fields: fields,
            cancellationToken: CancellationToken.None);

        var firstPage = await navigator.MoveNextAsync() ? navigator.Current : null;

        // Assert
        Assert.NotNull(firstPage);
        if (firstPage.Results?.Any() == true)
        {
            var catalog = firstPage.Results.First();
            Assert.NotNull(catalog.Title); // Запрошенное поле должно быть заполнено
        }
    }

    [Fact]
    public async Task GetByGroupAnchorNavigator_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 10,
            Direction = PagingDirection.FORWARD
        };

        // Act & Assert
        var navigator = _marketClient.GetByGroupAnchorNavigator<CatalogDto>(
            context: _groupContext,
            anchorConfiguration: anchorConfig,
            fields: null,
            cancellationToken: cancellationTokenSource.Token);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await foreach (var page in navigator)
            {
                // Должно выбросить исключение при первой итерации
            }
        });
    }

    [Fact]
    public async Task GetByGroupAnchorNavigator_Reset_ShouldRestartIteration()
    {
        var first = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        var second = await _marketClient.AddAsync(
            $"second {DateTime.UtcNow:yyyyMMddHHmmss}", false, _groupContext);
        // Arrange
        var anchorConfig = new AnchorConfiguration
        {
            Anchor = string.Empty,
            Count = 3,
            Direction = PagingDirection.FORWARD
        };

        try
        {
            var navigator = _marketClient.GetByGroupAnchorNavigator<CatalogDto>(
                context: _groupContext,
                anchorConfiguration: anchorConfig,
                fields: null,
                cancellationToken: CancellationToken.None);

            // Act - первая итерация
            var firstPageCount = 0;
            await foreach (var page in navigator)
            {
                firstPageCount += page.Results?.Count ?? 0;
                break; // Берём только первую страницу
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
        finally
        {
            await _marketClient.DeleteAsync(first, false, _groupContext);
            await _marketClient.DeleteAsync(second, false, _groupContext);
        }
    }

    #endregion

    #region Context Validation Tests

    [Theory]
    [InlineData("FriendRequestContext")]
    [InlineData("UserRequestContext")]
    [InlineData("ExplicitTokenRequestContext")]
    public async Task AllMethods_WithUnsupportedContext_ShouldThrowUnexpectedRequestContext(string contextType)
    {
        // Arrange
        var fakeContext = new FakeRequestContext(contextType);
        var catalogId = "YOUR_CATALOG_ID";

        // Act & Assert для каждого метода
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _marketClient.EditAsync(catalogId, "Title", false, fakeContext, "PhotoId", CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _marketClient.DeleteAsync(catalogId, false, fakeContext, CancellationToken.None));
        
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
            await _marketClient.ReorderAsync(catalogId, fakeContext, null, CancellationToken.None));
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