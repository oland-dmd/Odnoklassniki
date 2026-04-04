using Odnoklassniki.Exceptions;
using Odnoklassniki.Image;
using Odnoklassniki.Rest.ApiClients.PhotosV2;
using Odnoklassniki.Rest.RequestContexts;
using Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Odnoklassniki.ApiClient.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class PhotosV2ApiClientIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly PhotosV2ApiClient _photosV2Client;
    private readonly ImageClient _imageClient;

    public PhotosV2ApiClientIntegrationTests(OkApiTestFixture fixture)
    {
        _photosV2Client = new PhotosV2ApiClient(fixture.ClientCore);
        _imageClient = new ImageClient();
    }

    #region GetUploadUrlAsync (Получение URL для загрузки)

    [Fact]
    public async Task GetUploadUrlAsync_WithValidUserAlbum_ShouldReturnValidUploadUrl()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId; // Введите ID пользовательского альбома

        // Act
        var result = await _photosV2Client.GetUploadUrlAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            albumId: albumId,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.UploadUrl);
        Assert.NotNull(result.PhotoIds);
        Assert.NotEmpty(result.PhotoIds);
        Assert.Single(result.PhotoIds);
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithValidGroupAlbum_ShouldReturnValidUploadUrl()
    {
        // Arrange
        var albumId = TestSettings.GroupAlbumId; // Введите ID группового альбома

        // Act
        var result = await _photosV2Client.GetUploadUrlAsync(
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
            albumId: albumId,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.UploadUrl);
        Assert.NotNull(result.PhotoIds);
        Assert.NotEmpty(result.PhotoIds);
    }

    [Fact(Skip = "Нереализована возможность множественной загрузки")]
    public async Task GetUploadUrlAsync_WithMultiplePhotos_ShouldReturnMultiplePhotoIds()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId; // Введите ID пользовательского альбома
        var count = 5;

        // Act
        var result = await _photosV2Client.GetUploadUrlAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            albumId: albumId,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.PhotoIds);
        Assert.Equal(count, result.PhotoIds.Count);
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithInvalidAlbumId_ShouldThrowException()
    {
        // Arrange
        var invalidAlbumId = "INVALID_ALBUM_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.GetUploadUrlAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                albumId: invalidAlbumId,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var albumId = TestSettings.GroupAlbumId;
        var invalidGroupId = "INVALID_GROUP_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.GetUploadUrlAsync(
                new GroupRequestContext(TestSettings.AccessPair, new GroupId(invalidGroupId)),
                albumId: albumId,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";
        var albumId = TestSettings.UserAlbumId;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.GetUploadUrlAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = invalidToken }),
                albumId: albumId,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var albumId = TestSettings.UserAlbumId;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosV2Client.GetUploadUrlAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                albumId: albumId,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithEmptyAlbumId_ShouldReturnUserPhotoId()
    {
        // Arrange
        var count = 1;

        // Act
        var result = await _photosV2Client.GetUploadUrlAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(result.PhotoIds.Count, count);
        Assert.NotEmpty(result.UploadUrl);
    }

    [Fact(Skip = "Нереализована возможность множественной загрузки")]
    public async Task GetUploadUrlAsync_WithZeroCount_ShouldThrowException()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId;
        var count = 0; // Неверное значение
        var groupId = string.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _photosV2Client.GetUploadUrlAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                albumId: albumId,
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion

    #region CommitAsync (Подтверждение загрузки)

    
    private async Task<IDictionary<string, string>> UploadTestImage(IRequestContext context)
    {
        var uploadUrl = await _photosV2Client.GetUploadUrlAsync(context);
        await using var file = File.Open("./test.png",
            FileMode.Open);
        var token = await _imageClient.UploadImageAsync(uploadUrl.UploadUrl, file, CancellationToken.None);
        
        return token;
    }
    
    [Fact]
    public async Task CommitAsync_WithValidPhotoIdAndToken_ShouldReturnCommitData()
    {
        // Arrange
        var tokenResponse = await UploadTestImage(new ExplicitTokenRequestContext(TestSettings.AccessPair));
        
        var photoId = tokenResponse.Keys.First(); // Введите ID загруженной фотографии
        var token = tokenResponse.Values.First();        // Введите токен загрузки
        var comment = $"Integration Test Comment {DateTime.UtcNow:yyyyMMddHHmmss}";

        // Act
        var result = await _photosV2Client.CommitAsync(
            comment: comment,
            photoId: photoId,
            token: token,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var commitData = result.FirstOrDefault();
        Assert.NotNull(commitData);
        Assert.NotNull(commitData.Id);
        Assert.NotNull(commitData.Status);
    }

    [Fact]
    public async Task CommitAsync_WithEmptyComment_ShouldReturnCommitData()
    {
        // Arrange
        var tokenResponse = await UploadTestImage(new ExplicitTokenRequestContext(TestSettings.AccessPair));
        
        var photoId = tokenResponse.Keys.First(); // Введите ID загруженной фотографии
        var token = tokenResponse.Values.First();      // Введите токен загрузки
        var comment = string.Empty;

        // Act
        var result = await _photosV2Client.CommitAsync(
            comment: comment,
            photoId: photoId,
            token: token,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task CommitAsync_WithInvalidPhotoId_ShouldThrowException()
    {
        // Arrange
        var invalidPhotoId = "INVALID_PHOTO_ID_12345";
        var token = "YOUR_UPLOAD_TOKEN"; // Введите валидный токен
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: invalidPhotoId,
                token: token,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task CommitAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var photoId = TestSettings.UserAlbumPhotoId;       // Введите валидный ID фотографии
        var invalidToken = "INVALID_TOKEN";  // Неверный токен
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: photoId,
                token: invalidToken,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task CommitAsync_WithInvalidAccessToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_ACCESS_TOKEN_12345";
        var photoId = "YOUR_PHOTO_ID";
        var token = "YOUR_UPLOAD_TOKEN";
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: photoId,
                token: token,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with {  AccessToken = invalidToken }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task CommitAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var photoId = "YOUR_PHOTO_ID";
        var token = "YOUR_UPLOAD_TOKEN";
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: photoId,
                token: token,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task CommitAsync_WithNullPhotoId_ShouldThrowArgumentNullException()
    {
        // Arrange
        string photoId = null;
        var token = "YOUR_UPLOAD_TOKEN";
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: photoId,
                token: token,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task CommitAsync_WithNullToken_ShouldThrowArgumentNullException()
    {
        // Arrange
        var photoId = TestSettings.UserAlbumPhotoId;
        string token = null;
        var comment = "Test comment";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _photosV2Client.CommitAsync(
                comment: comment,
                photoId: photoId,
                token: token,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion
}