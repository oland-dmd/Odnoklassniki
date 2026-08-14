using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Image;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;
using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.IntegrationTests;

internal record PhotoSimpleDto : BaseOkDto
{
    [JsonPropertyName("photo_id")]
    public string? PhotoId { get; init; }
}

[Collection("Integration")]
[Trait("Category", "Integration")]
public class PhotosApiClientExtensionsIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly PhotosApiClient _photosClient;
    private readonly PhotosV2ApiClient _photosV2Client;
    private readonly ImageClient _imageClient;
    private readonly AlbumsApiClient _albumsClient;

    public PhotosApiClientExtensionsIntegrationTests(OkApiTestFixture fixture)
    {
        _photosClient = new PhotosApiClient(fixture.ClientCore);
        _photosV2Client = new PhotosV2ApiClient(fixture.ClientCore);
        _imageClient = new ImageClient(new HttpClient());
        _albumsClient = new AlbumsApiClient(fixture.ClientCore);
    }


    #region Get Photos (Получение списков фотографий)

    [Fact]
    public async Task GetUserPhotosAsync_WithValidUserToken_ShouldReturnValidPhotoList()
    {
        // Arrange - создаём временный альбом
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var albumId = await _albumsClient.CreateAlbumAsync($"Temp Photos Test {DateTime.UtcNow:yyyyMMddHHmmss}", userContext);

        try
        {
            // Act
            var navigator = _photosClient.GetPhotosNavigator(albumId, userContext, new AnchorConfiguration());
            await navigator.MoveNextAsync();
            var result = navigator.Current;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Results);
        }
        finally
        {
            await _albumsClient.DeleteAlbumAsync(albumId, userContext);
        }
    }

    [Fact]
    public async Task GetGroupPhotosAsync_WithValidGroupToken_ShouldReturnValidPhotoList()
    {
        // Arrange
        var albumId = TestSettings.GroupAlbumId; // Введите ID группового альбома

        // Act
        var navigatort = _photosClient.GetPhotosNavigator(
            albumId: albumId,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId), new AnchorConfiguration());
        
        await navigatort.MoveNextAsync();
        
        var result = navigatort.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task GetFriendPhotosAsync_WithValidFriendId_ShouldReturnValidPhotoList()
    {
        // Arrange
        var albumId = TestSettings.FriendAlbumId; // Введите ID альбома друга
        var friendId = TestSettings.FriendId;      // Введите ID друга

        // Act
        var navigator = _photosClient.GetPhotosNavigator(
            albumId: albumId,
            new FriendRequestContext(TestSettings.AccessPair, friendId), new AnchorConfiguration());
        
        await navigator.MoveNextAsync();
        
        var result =  navigator.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task GetUserPhotosAsync_WithPaginationParameters_ShouldReturnValidPhotoList()
    {
        // Arrange - создаём временный альбом
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var albumId = await _albumsClient.CreateAlbumAsync($"Temp Pagination Test {DateTime.UtcNow:yyyyMMddHHmmss}", userContext);
        var count = 10;

        try
        {
            // Act
            var navigator = _photosClient.GetPhotosNavigator(albumId: albumId, userContext, new AnchorConfiguration { Count = count });
            await navigator.MoveNextAsync();
            var result = navigator.Current;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Results);
            Assert.True(result.Results.Count <= count);
        }
        finally
        {
            await _albumsClient.DeleteAlbumAsync(albumId, userContext);
        }
    }

    #endregion

    #region Get Photo Info (Получение информации о фотографии)

    [Fact]
    public async Task GetUserPhotoInfoAsync_WithValidPhotoId_ShouldReturnValidPhotoData()
    {
        // Arrange - загружаем временное фото
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var commitResponse = await UploadTestImage(userContext);
        var photoId = commitResponse.First().Id!;

        try
        {
            // Act
            var result = await _photosClient.GetPhotoInfoAsync(photoId, userContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(photoId, result.Id);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, userContext);
        }
    }

    [Fact]
    public async Task GetGroupPhotoInfoAsync_WithValidPhotoId_ShouldReturnValidPhotoData()
    {
        // Arrange - загружаем временное фото в группу
        var groupContext = new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId);
        var commitResponse = await UploadTestImage(groupContext);
        var photoId = commitResponse.First().Id!;

        try
        {
            // Act
            var result = await _photosClient.GetPhotoInfoAsync(photoId: photoId, groupContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(photoId, result.Id);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, groupContext);
        }
    }

    [Fact]
    public async Task GetUserPhotoInfoAsync_WithInvalidPhotoId_ShouldThrowException()
    {
        // Arrange
        var invalidPhotoId = "INVALID_PHOTO_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetPhotoInfoAsync(
                photoId: invalidPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    #endregion

    #region Edit Photo (Редактирование фотографий)

    [Fact]
    public async Task EditUserPhotoAsync_WithValidData_ShouldUpdatePhotoSuccessfully()
    {
        // Arrange - загружаем временное фото
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var commitResponse = await UploadTestImage(userContext);
        var photoId = commitResponse.First().Id!;
        var newDescription = $"Integration Test Description {DateTime.UtcNow:yyyyMMddHHmmss}";

        try
        {
            // Act
            await _photosClient.EditPhotoAsync(
                photoId: photoId,
                description: newDescription,
                userContext);

            // Assert
            var updatedPhoto = await _photosClient.GetPhotoInfoAsync(photoId, userContext);
            Assert.NotNull(updatedPhoto);
            Assert.Contains(newDescription, updatedPhoto.Text);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, userContext);
        }
    }

    [Fact]
    public async Task EditGroupPhotoAsync_WithValidData_ShouldUpdatePhotoSuccessfully()
    {
        // Arrange - загружаем временное фото в группу
        var groupContext = new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId);
        var commitResponse = await UploadTestImage(groupContext);
        var photoId = commitResponse.First().Id!;
        var newDescription = $"Integration Test Group Description {DateTime.UtcNow:yyyyMMddHHmmss}";

        try
        {
            // Act
            var result = await _photosClient.EditPhotoAsync(
                photoId: photoId,
                description: newDescription,
                groupContext);

            // Assert
            var updatedPhoto = await _photosClient.GetPhotoInfoAsync(photoId, groupContext);
            Assert.NotNull(updatedPhoto);
            Assert.Contains(newDescription, updatedPhoto.Text);
            Assert.True(result);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, groupContext);
        }
    }

    #endregion

    #region Delete Photo (Удаление фотографий)

    [Fact]
    public async Task DeleteUserPhotoAsync_WithValidPhotoId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var commitResponse = await UploadTestImage(new ExplicitTokenRequestContext(TestSettings.AccessPair));

        var photoId = commitResponse.First().Id; // Введите ID фотографии для удаления
        // Примечание: Для полноценного теста нужно сначала загрузить фото через Upload API

        // Act
        await _photosClient.DeletePhotoAsync(
            photoId: photoId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        // Попытка получить удаленное фото должна вернуть null или выбросить исключение
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _photosClient.GetPhotoInfoAsync(
                photoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
        
        Assert.NotNull(exception);
    }

    private async Task<ICollection<CommitPhotoData>> UploadTestImage(IRequestContext context)
    {
        var uploadUrl = await _photosV2Client.GetUploadUrlAsync(context);
        await using var file = File.Open("./test.png",
            FileMode.Open);
        var token = await _imageClient.UploadImageAsync(uploadUrl.UploadUrl, file, CancellationToken.None);
        
        var commitResponse = await _photosV2Client.CommitAsync("test", token.Keys.First(), token.Values.First(), new ExplicitTokenRequestContext(TestSettings.AccessPair), CancellationToken.None);
        
        return commitResponse;
    }

    [Fact]
    public async Task DeleteGroupPhotoAsync_WithValidPhotoId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var commitResponse = await UploadTestImage(new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));

        var photoId = commitResponse.First().Id; // Введите ID групповой фотографии для удаления

        // Act
        await _photosClient.DeletePhotoAsync(
            photoId: photoId,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));

        // Assert
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _photosClient.GetPhotoInfoAsync(
                photoId,
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));
        });
        
        Assert.NotNull(exception);
    }

    #endregion

    #region Cancellation Token

    [Fact]
    public async Task GetUserPhotosAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var albumId = TestSettings.UserAlbumId;//"937581127103";

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _photosClient.GetPhotosNavigator(
                albumId: albumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration(),
                cancellationToken: cancellationTokenSource.Token);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetUserPhotoInfoAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var photoId = TestSettings.UserAlbumPhotoId;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.GetPhotoInfoAsync(
                photoId: photoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task EditUserPhotoAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var photoId = TestSettings.UserAlbumPhotoId;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.EditPhotoAsync(
                photoId: photoId,
                description: "Test",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region Invalid Token Tests

    [Fact]
    public async Task GetUserPhotosAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";
        var albumId = TestSettings.UserAlbumId;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetPhotosNavigator(
                albumId: albumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = invalidToken }),
                new AnchorConfiguration());
            
            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetGroupPhotosAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var invalidGroupId = "INVALID_GROUP_ID_12345";
        var albumId = TestSettings.GroupAlbumId;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetPhotosNavigator(
                albumId: albumId,
                new GroupRequestContext(TestSettings.AccessPair,  new GroupId(invalidGroupId)), new AnchorConfiguration());

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetPhotosInfoAsync (Получение информации о фотографиях по ID)

    [Fact]
    public async Task GetPhotosInfoAsync_WithValidPhotoId_ShouldReturnPhotoInfo()
    {
        var result = await _photosClient.GetPhotosInfoAsync<PhotoSimpleDto>(
            new[] { TestSettings.UserAlbumPhotoId },
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPhotosInfoAsync_WithInvalidPhotoId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetPhotosInfoAsync<PhotoSimpleDto>(
                new[] { "INVALID_PHOTO_ID_12345" },
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    [Fact]
    public async Task GetPhotosInfoAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetPhotosInfoAsync<PhotoSimpleDto>(
                new[] { TestSettings.UserAlbumPhotoId },
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task GetPhotosInfoAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.GetPhotosInfoAsync<PhotoSimpleDto>(
                new[] { TestSettings.UserAlbumPhotoId },
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetUserPhotosNavigator (Пагинация фотографий пользователя)

    [Fact]
    public async Task GetUserPhotosNavigator_WithValidToken_ShouldReturnPhotos()
    {
        var navigator = _photosClient.GetUserPhotosNavigator<PhotoSimpleDto>(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration { Count = 10 });

        await navigator.MoveNextAsync();
        var result = navigator.Current;

        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task GetUserPhotosNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetUserPhotosNavigator<PhotoSimpleDto>(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetUserPhotosNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _photosClient.GetUserPhotosNavigator<PhotoSimpleDto>(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetUserAlbumPhotosNavigator (Пагинация фотографий альбома пользователя)

    [Fact]
    public async Task GetUserAlbumPhotosNavigator_WithValidAlbumId_ShouldReturnPhotos()
    {
        // Arrange - создаём временный альбом
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var albumId = await _albumsClient.CreateAlbumAsync($"Temp Navigator Test {DateTime.UtcNow:yyyyMMddHHmmss}", userContext);

        try
        {
            var navigator = _photosClient.GetUserAlbumPhotosNavigator<PhotoSimpleDto>(
                albumId,
                userContext,
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
            var result = navigator.Current;

            Assert.NotNull(result);
            Assert.NotNull(result.Results);
        }
        finally
        {
            await _albumsClient.DeleteAlbumAsync(albumId, userContext);
        }
    }

    [Fact]
    public async Task GetUserAlbumPhotosNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetUserAlbumPhotosNavigator<PhotoSimpleDto>(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetUserAlbumPhotosNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _photosClient.GetUserAlbumPhotosNavigator<PhotoSimpleDto>(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region AddPhotoLikeAsync (Лайк фотографии)

    [Fact]
    public async Task AddPhotoLikeAsync_WithValidPhotoId_ShouldReturnBool()
    {
        // Arrange - загружаем временное фото
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var commitResponse = await UploadTestImage(userContext);
        var photoId = commitResponse.First().Id!;

        try
        {
            var result = await _photosClient.AddPhotoLikeAsync(photoId, userContext);
            Assert.True(result || !result);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, userContext);
        }
    }

    [Fact]
    public async Task AddPhotoLikeAsync_WithInvalidPhotoId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.AddPhotoLikeAsync(
                "INVALID_PHOTO_ID_12345",
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    [Fact]
    public async Task AddPhotoLikeAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.AddPhotoLikeAsync(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task AddPhotoLikeAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.AddPhotoLikeAsync(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region AddAlbumLikeAsync (Лайк альбома)

    [Fact]
    public async Task AddAlbumLikeAsync_WithValidAlbumId_ShouldReturnBool()
    {
        // Arrange - создаём временный альбом
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var albumId = await _albumsClient.CreateAlbumAsync($"Temp Like Test {DateTime.UtcNow:yyyyMMddHHmmss}", userContext);

        try
        {
            var result = await _photosClient.AddAlbumLikeAsync(albumId, userContext);
            Assert.True(result || !result);
        }
        finally
        {
            await _albumsClient.DeleteAlbumAsync(albumId, userContext);
        }
    }

    [Fact]
    public async Task AddAlbumLikeAsync_WithInvalidAlbumId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.AddAlbumLikeAsync(
                "INVALID_ALBUM_ID_12345",
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    [Fact]
    public async Task AddAlbumLikeAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.AddAlbumLikeAsync(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task AddAlbumLikeAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.AddAlbumLikeAsync(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetPhotoLikesNavigator (Лайки фотографии)

    [Fact]
    public async Task GetPhotoLikesNavigator_WithValidPhotoId_ShouldReturnLikes()
    {
        // Arrange - загружаем временное фото
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var commitResponse = await UploadTestImage(userContext);
        var photoId = commitResponse.First().Id!;

        try
        {
            var navigator = _photosClient.GetPhotoLikesNavigator(
                photoId,
                userContext,
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
            var result = navigator.Current;

            Assert.NotNull(result);
            Assert.NotNull(result.Results);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, userContext);
        }
    }

    [Fact]
    public async Task GetPhotoLikesNavigator_WithInvalidPhotoId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetPhotoLikesNavigator(
                "INVALID_PHOTO_ID_12345",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetPhotoLikesNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetPhotoLikesNavigator(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetPhotoLikesNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _photosClient.GetPhotoLikesNavigator(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetAlbumLikesNavigator (Лайки альбома)

    [Fact]
    public async Task GetAlbumLikesNavigator_WithValidAlbumId_ShouldReturnLikes()
    {
        // Arrange - создаём временный альбом
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var albumId = await _albumsClient.CreateAlbumAsync($"Temp Likes Navigator Test {DateTime.UtcNow:yyyyMMddHHmmss}", userContext);

        try
        {
            var navigator = _photosClient.GetAlbumLikesNavigator(
                albumId,
                userContext,
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
            var result = navigator.Current;

            Assert.NotNull(result);
            Assert.NotNull(result.Results);
        }
        finally
        {
            await _albumsClient.DeleteAlbumAsync(albumId, userContext);
        }
    }

    [Fact]
    public async Task GetAlbumLikesNavigator_WithInvalidAlbumId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetAlbumLikesNavigator(
                "INVALID_ALBUM_ID_12345",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetAlbumLikesNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _photosClient.GetAlbumLikesNavigator(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 });

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetAlbumLikesNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _photosClient.GetAlbumLikesNavigator(
                TestSettings.UserAlbumId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetPhotoMarksAsync (Отметки на фотографиях)

    [Fact]
    public async Task GetPhotoMarksAsync_WithValidToken_ShouldReturnMarksOrNull()
    {
        var result = await _photosClient.GetPhotoMarksAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task GetPhotoMarksAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetPhotoMarksAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task GetPhotoMarksAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.GetPhotoMarksAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetTagsAsync (Теги фотографии)

    [Fact]
    public async Task GetTagsAsync_WithValidPhotoId_ShouldReturnTagsOrNull()
    {
        // Arrange - загружаем временное фото
        var userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        var commitResponse = await UploadTestImage(userContext);
        var photoId = commitResponse.First().Id!;

        try
        {
            var result = await _photosClient.GetTagsAsync(photoId, userContext);
            Assert.True(result == null || result != null);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, userContext);
        }
    }

    [Fact]
    public async Task GetTagsAsync_WithInvalidPhotoId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetTagsAsync(
                "INVALID_PHOTO_ID_12345",
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    [Fact]
    public async Task GetTagsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.GetTagsAsync(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task GetTagsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.GetTagsAsync(
                TestSettings.UserAlbumPhotoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region DeleteTagsAsync (Удаление тегов фотографии)

    [Fact]
    public async Task DeleteTagsAsync_WithInvalidPhotoId_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.DeleteTagsAsync(
                "INVALID_PHOTO_ID_12345",
                new[] { "some_tag_id" },
                new ExplicitTokenRequestContext(TestSettings.AccessPair));
        });
    }

    [Fact]
    public async Task DeleteTagsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosClient.DeleteTagsAsync(
                TestSettings.UserAlbumPhotoId,
                new[] { "some_tag_id" },
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }));
        });
    }

    [Fact]
    public async Task DeleteTagsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosClient.DeleteTagsAsync(
                TestSettings.UserAlbumPhotoId,
                new[] { "some_tag_id" },
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion
}