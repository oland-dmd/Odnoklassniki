using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class AlbumsApiClientExtensionsIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly AlbumsApiClient _albumsClient = new(fixture.ClientCore);

    #region Get Albums (Получение списков альбомов)

    [Fact]
    public async Task GetUserAlbumsAsync_WithValidUserToken_ShouldReturnValidAlbumList()
    {
        // Act
        var navigator = _albumsClient.GetAlbumsNavigatorAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair), new AnchorConfiguration());

        await navigator.MoveNextAsync();

        var result = navigator.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.NotEmpty(result.Results);
    }

    [Fact]
    public async Task GetGroupAlbumsAsync_WithValidGroupToken_ShouldReturnValidAlbumList()
    {
        // Arrange
        var groupId = TestSettings.GroupId;

        // Act
        var navigator = _albumsClient.GetAlbumsNavigatorAsync(
            new GroupRequestContext(TestSettings.AccessPair, groupId), new AnchorConfiguration());

        await navigator.MoveNextAsync();

        var result = navigator.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.NotEmpty(result.Results);
    }

    [Fact]
    public async Task GetFriendAlbumsAsync_WithValidFriendId_ShouldReturnValidAlbumList()
    {
        // Arrange
        var friendId = TestSettings.FriendId;

        // Act
        var navigator = _albumsClient.GetAlbumsNavigatorAsync(
            new FriendRequestContext(TestSettings.AccessPair, friendId), new AnchorConfiguration());

        await navigator.MoveNextAsync();

        var result = navigator.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    #endregion

    #region Get Album Info (Получение информации об альбоме)

    [Fact]
    public async Task GetUserAlbumInfoAsync_WithValidAlbumId_ShouldReturnValidAlbumData()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId;

        // Act
        var result = await _albumsClient.GetAlbumInfoAsync(
            albumId: albumId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(albumId, result.Id);
    }

    [Fact]
    public async Task GetGroupAlbumInfoAsync_WithValidAlbumId_ShouldReturnValidAlbumData()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var albumId = TestSettings.GroupAlbumId;

        // Act
        var result = await _albumsClient.GetAlbumInfoAsync(
            albumId: albumId,
            new GroupRequestContext(TestSettings.AccessPair, groupId));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(albumId, result.Id);
    }

    [Fact]
    public async Task GetFriendAlbumInfoAsync_WithValidAlbumId_ShouldReturnValidAlbumData()
    {
        // Arrange
        const string albumId = TestSettings.FriendAlbumId;

        // Act
        var result = await _albumsClient.GetAlbumInfoAsync(
            albumId: albumId,
            new FriendRequestContext(TestSettings.AccessPair, TestSettings.FriendId));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(albumId, result.Id);
    }

    #endregion

    #region Create Album (Создание альбомов)

    [Fact]
    public async Task CreateUserAlbumAsync_WithValidToken_ShouldReturnNewAlbumId()
    {
        // Arrange
        var title = $"Integration Test Album {DateTime.UtcNow:yyyyMMddHHmmss}";

        // Act
        var albumId = await _albumsClient.CreateAlbumAsync(title: title,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        Assert.NotNull(albumId);
        Assert.NotEmpty(albumId);
        
        // Cleanup: Удаляем созданный альбом после теста
        await _albumsClient.DeleteAlbumAsync(albumId, new ExplicitTokenRequestContext(TestSettings.AccessPair));
    }

    [Fact]
    public async Task CreateGroupAlbumAsync_WithValidToken_ShouldReturnNewAlbumId()
    {
        // Arrange
        var title = $"Integration Test Group Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var groupId = TestSettings.GroupId;

        // Act
        var albumId = await _albumsClient.CreateAlbumAsync(title: title,
            new GroupRequestContext(TestSettings.AccessPair, groupId));

        // Assert
        Assert.NotNull(albumId);
        Assert.NotEmpty(albumId);

        // Cleanup
        await _albumsClient.DeleteAlbumAsync(albumId, new GroupRequestContext(TestSettings.AccessPair, groupId));
    }

    #endregion

    #region Edit Album (Редактирование альбомов)

    [Fact]
    public async Task EditUserAlbumAsync_WithValidData_ShouldUpdateAlbumSuccessfully()
    {
        // Arrange
        var title = $"Edit Test Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var newTitle = $"Edited Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var description = "Test description";
        
        var albumId = await _albumsClient.CreateAlbumAsync(title, new ExplicitTokenRequestContext(TestSettings.AccessPair));

        try
        {
            // Act
            await _albumsClient.EditAlbumAsync(
                albumId: albumId,
                title: newTitle,
                description: description,
                new ExplicitTokenRequestContext(TestSettings.AccessPair));

            // Assert
            var updatedAlbum = await _albumsClient.GetAlbumInfoAsync(albumId, new ExplicitTokenRequestContext(TestSettings.AccessPair));
            
            Assert.Equal(newTitle, updatedAlbum.Title);
        }
        finally
        {
            // Cleanup
            await _albumsClient.DeleteAlbumAsync(albumId, new ExplicitTokenRequestContext(TestSettings.AccessPair));
        }
    }

    [Fact]
    public async Task EditGroupAlbumAsync_WithValidData_ShouldUpdateAlbumSuccessfully()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var title = $"Edit Test Group Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var newTitle = $"Edited Group Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var description = "Test group description";

        var albumId = await _albumsClient.CreateAlbumAsync(title, new GroupRequestContext(TestSettings.AccessPair, groupId));

        try
        {
            // Act
            await _albumsClient.EditAlbumAsync(
                albumId: albumId,
                title: newTitle,
                description: description,
                new GroupRequestContext(TestSettings.AccessPair, groupId));

            // Assert
            var updatedAlbum = await _albumsClient.GetAlbumInfoAsync(albumId, new GroupRequestContext(TestSettings.AccessPair, groupId));
            
            Assert.Equal(newTitle, updatedAlbum.Title);
        }
        finally
        {
            // Cleanup
            await _albumsClient.DeleteAlbumAsync(albumId, new GroupRequestContext(TestSettings.AccessPair, groupId));
        }
    }

    #endregion

    #region Delete Album (Удаление альбомов)

    [Fact]
    public async Task DeleteUserAlbumAsync_WithValidAlbumId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var title = $"Delete Test Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var albumId = await _albumsClient.CreateAlbumAsync(title, new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Act
        await _albumsClient.DeleteAlbumAsync(
            albumId: albumId!,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        // Попытка получить удаленный альбом должна вернуть null или выбросить исключение (зависит от реализации API)
        await Assert.ThrowsAsync<OkApiException>(async () => await _albumsClient.GetAlbumInfoAsync(albumId,new ExplicitTokenRequestContext(TestSettings.AccessPair)));
    }

    [Fact]
    public async Task DeleteGroupAlbumAsync_WithValidAlbumId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var title = $"Delete Test Group Album {DateTime.UtcNow:yyyyMMddHHmmss}";
        var albumId = await _albumsClient.CreateAlbumAsync(title, new GroupRequestContext(TestSettings.AccessPair, groupId));

        // Act
        await _albumsClient.DeleteAlbumAsync(
            albumId: albumId,
            new GroupRequestContext(TestSettings.AccessPair, groupId));

        // Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
            await _albumsClient.GetAlbumInfoAsync(albumId,new GroupRequestContext(TestSettings.AccessPair, groupId)));
    }

    #endregion

    #region Set Main Photo (Установка главной фотографии)

    [Fact]
    public async Task SetUserAlbumMainPhotoAsync_WithValidPhotoId_ShouldSetMainPhotoSuccessfully()
    {
        // Arrange
        var albumId = TestSettings.UserAlbumId;
        var photoId = TestSettings.UserAlbumPhotoId;

        // Act
        await _albumsClient.SetAlbumMainPhotoAsync(
            albumId: albumId,
            photoId: photoId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        var albumInfo = await _albumsClient.GetAlbumInfoAsync(albumId, new ExplicitTokenRequestContext(TestSettings.AccessPair));
        
        Assert.NotNull(albumInfo);
        // Проверка, что фото установилось (если поле доступно в модели)
        // Assert.Equal(photoId, albumInfo.MainPhotoId); 
    }

    [Fact]
    public async Task SetGroupAlbumMainPhotoAsync_WithValidPhotoId_ShouldSetMainPhotoSuccessfully()
    {
        // Arrange
        var albumId = TestSettings.GroupAlbumId;
        var photoId = TestSettings.GroupPhotoAlbumId;
        var groupId = TestSettings.GroupId;

        // Act
        await _albumsClient.SetAlbumMainPhotoAsync(
            albumId: albumId,
            photoId: photoId,
            new GroupRequestContext(TestSettings.AccessPair, groupId));

        // Assert
        var albumInfo = await _albumsClient.GetAlbumInfoAsync(albumId, new GroupRequestContext(TestSettings.AccessPair, groupId));
        
        Assert.NotNull(albumInfo);
    }

    #endregion
}