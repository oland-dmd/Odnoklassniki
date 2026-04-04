using Odnoklassniki.Exceptions;
using Odnoklassniki.Image;
using Odnoklassniki.Rest.AnchorNavigators;
using Odnoklassniki.Rest.ApiClients.Photos;
using Odnoklassniki.Rest.ApiClients.PhotosV2;
using Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;
using Odnoklassniki.Rest.RequestContexts;
using Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.ApiClient.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class PhotosApiClientExtensionsIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly PhotosApiClient _photosClient;
    private readonly PhotosV2ApiClient _photosV2Client;
    private readonly ImageClient _imageClient;

    public PhotosApiClientExtensionsIntegrationTests(OkApiTestFixture fixture)
    {
        _photosClient = new PhotosApiClient(fixture.ClientCore);
        _photosV2Client = new PhotosV2ApiClient(fixture.ClientCore);
        _imageClient = new ImageClient();
    }


    #region Get Photos (Получение списков фотографий)

    [Fact]
    public async Task GetUserPhotosAsync_WithValidUserToken_ShouldReturnValidPhotoList()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId;

        // Act
        var navigator = _photosClient.GetPhotosNavigator(
            albumId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair), new AnchorConfiguration());

        await navigator.MoveNextAsync();
        
        var result = navigator.Current;
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
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
        // Arrange
        var albumId = TestSettings.UserAlbumId;
        var count = 10;
        var anchor = string.Empty;

        // Act
        var navigator = _photosClient.GetPhotosNavigator(
            albumId: albumId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair), new AnchorConfiguration()
            {
                Count = count
            });

        await navigator.MoveNextAsync();

        var result = navigator.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.True(result.Results.Count <= count);
    }

    #endregion

    #region Get Photo Info (Получение информации о фотографии)

    [Fact]
    public async Task GetUserPhotoInfoAsync_WithValidPhotoId_ShouldReturnValidPhotoData()
    {
        // Arrange
        var photoId = TestSettings.UserAlbumPhotoId; // Введите ID пользовательской фотографии

        // Act
        var result = await _photosClient.GetPhotoInfoAsync(
            photoId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(photoId, result.Id);
    }

    [Fact]
    public async Task GetGroupPhotoInfoAsync_WithValidPhotoId_ShouldReturnValidPhotoData()
    {
        // Arrange
        var photoId = TestSettings.GroupPhotoAlbumId; // Введите ID групповой фотографии
        var groupId = TestSettings.GroupId;       // Введите ID группы

        // Act
        var result = await _photosClient.GetPhotoInfoAsync(
            photoId: photoId,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(photoId, result.Id);
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
        // Arrange
        var photoId = TestSettings.UserAlbumPhotoId;
        var newDescription = $"Integration Test Description {DateTime.UtcNow:yyyyMMddHHmmss}";

        // Act
        await _photosClient.EditPhotoAsync(
            photoId: photoId,
            description: newDescription,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        var updatedPhoto = await _photosClient.GetPhotoInfoAsync(
            photoId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));
        
        Assert.NotNull(updatedPhoto);
        Assert.Contains(newDescription, updatedPhoto.Text);
    }

    [Fact]
    public async Task EditGroupPhotoAsync_WithValidData_ShouldUpdatePhotoSuccessfully()
    {
        // Arrange
        var photoId = TestSettings.GroupPhotoAlbumId; // Введите ID групповой фотографии
        var groupId = TestSettings.GroupId;       // Введите ID группы
        var newDescription = $"Integration Test Group Description {DateTime.UtcNow:yyyyMMddHHmmss}";

        // Act
        var result = await _photosClient.EditPhotoAsync(
            photoId: photoId,
            description: newDescription,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));

        // Assert
        var updatedPhoto = await _photosClient.GetPhotoInfoAsync(
            photoId,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId));
        
        Assert.NotNull(updatedPhoto);
        Assert.Contains(newDescription, updatedPhoto.Text);
        Assert.True(result);
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
}