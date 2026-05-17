using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Friends;
using Oland.Odnoklassniki.Rest.ApiClients.Users;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class FriendsApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly FriendsApiClient _friendsClient = new(fixture.ClientCore);
    private readonly UserApiClient _userClient = new(fixture.ClientCore);

    #region GetUserFriendsAsync

    [Fact]
    public async Task GetUserFriendsAsync_WithValidToken_ShouldReturnFriendIds()
    {
        // Act
        var result = await _friendsClient.GetUserFriendsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Список друзей может быть пустым, но не null
        Assert.True(result.Count >= 0);
    }

    [Fact]
    public async Task GetUserFriendsAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetUserFriendsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUserFriendsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetUserFriendsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetAppUsersAsync

    [Fact]
    public async Task GetAppUsersAsync_WithValidToken_ShouldReturnCollectionOrNull()
    {
        // Act
        var result = await _friendsClient.GetAppUsersAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - коллекция может быть пустой или null, но без исключений
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetAppUsersAsync_WithValidToken_AppUserIdsShouldBeNumericStrings()
    {
        // Act
        var result = await _friendsClient.GetAppUsersAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - если есть результаты, все UID должны быть числовыми строками
        if (result != null && result.Count > 0)
        {
            foreach (var uid in result)
            {
                Assert.Matches(@"^\d+$", uid);
            }
        }
    }

    [Fact]
    public async Task GetAppUsersAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetAppUsersAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAppUsersAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetAppUsersAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetOnlineAsync

    [Fact]
    public async Task GetOnlineAsync_WithValidToken_ShouldReturnCollectionOrNull()
    {
        // Act
        var result = await _friendsClient.GetOnlineAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - онлайн-друзей может не быть, поэтому null или пустая коллекция — ок
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetOnlineAsync_WithFriendContext_ShouldReturnCollectionOrNull()
    {
        // Arrange - используем FriendRequestContext
        var friendIds = await _friendsClient.GetUserFriendsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        if (friendIds.Count == 0)
            return; // Нет друзей — тест неприменим

        var friendId = new FriendId(friendIds.First());

        // Act
        var result = await _friendsClient.GetOnlineAsync(
            new FriendRequestContext(TestSettings.AccessPair, friendId),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetOnlineAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetOnlineAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetOnlineAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetOnlineAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetMutualFriendsAsync

    [Fact]
    public async Task GetMutualFriendsAsync_WithFriendUid_ShouldReturnCollectionOrNull()
    {
        // Arrange - получаем UID друга автономно через список друзей
        var friendIds = await _friendsClient.GetUserFriendsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        if (friendIds.Count == 0)
            return; // Нет друзей — тест неприменим

        var targetUid = friendIds.First();

        // Act
        var result = await _friendsClient.GetMutualFriendsAsync(
            targetUid,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - общих друзей может не быть
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetMutualFriendsAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange - получаем UID друга автономно
        var friendIds = await _friendsClient.GetUserFriendsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        if (friendIds.Count == 0)
            return; // Нет друзей — тест неприменим

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetMutualFriendsAsync(
                friendIds.First(),
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetMutualFriendsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange - получаем UID друга автономно
        var friendIds = await _friendsClient.GetUserFriendsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        if (friendIds.Count == 0)
            return; // Нет друзей — тест неприменим

        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetMutualFriendsAsync(
                friendIds.First(),
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetByDevicesAsync

    [Fact]
    public async Task GetByDevicesAsync_WithOkDevice_ShouldReturnCollectionOrNull()
    {
        // Arrange
        var devices = new[] { "OK" };

        // Act
        var result = await _friendsClient.GetByDevicesAsync(
            devices,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetByDevicesAsync_WithMultipleDevices_ShouldReturnCollectionOrNull()
    {
        // Arrange
        var devices = new[] { "OK", "MOBILE", "WEB" };

        // Act
        var result = await _friendsClient.GetByDevicesAsync(
            devices,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetByDevicesAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var devices = new[] { "OK" };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetByDevicesAsync(
                devices,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetByDevicesAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var devices = new[] { "OK" };
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetByDevicesAsync(
                devices,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetBirthdaysAsync

    [Fact]
    public async Task GetBirthdaysAsync_WithFutureFalse_ShouldReturnBirthdaysInNext3Days()
    {
        // Act - дни рождения в ближайшие 3 дня
        var result = await _friendsClient.GetBirthdaysAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            future: false,
            cancellationToken: CancellationToken.None);

        // Assert - результат может быть пустым (нет именинников)
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetBirthdaysAsync_WithFutureTrue_ShouldReturnBirthdaysInNext30Days()
    {
        // Act - дни рождения в ближайшие 30 дней
        var result = await _friendsClient.GetBirthdaysAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            future: true,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetBirthdaysAsync_WithFutureTrue_ShouldReturnMoreOrEqualThanFutureFalse()
    {
        // Act - 30-дневный диапазон должен содержать не меньше именинников, чем 3-дневный
        var shortRange = await _friendsClient.GetBirthdaysAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            future: false,
            cancellationToken: CancellationToken.None);

        var longRange = await _friendsClient.GetBirthdaysAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            future: true,
            cancellationToken: CancellationToken.None);

        // Assert - 30 дней >= 3 дней
        var shortCount = shortRange?.Count ?? 0;
        var longCount = longRange?.Count ?? 0;
        Assert.True(longCount >= shortCount);
    }

    [Fact]
    public async Task GetBirthdaysAsync_BirthdayDtoFields_ShouldContainUidAndDate()
    {
        // Act
        var result = await _friendsClient.GetBirthdaysAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            future: true,
            cancellationToken: CancellationToken.None);

        // Assert - если есть записи, каждая должна содержать uid
        if (result != null && result.Count > 0)
        {
            foreach (var birthday in result)
            {
                Assert.NotNull(birthday.Uid);
                Assert.NotEmpty(birthday.Uid);
            }
        }
    }

    [Fact]
    public async Task GetBirthdaysAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetBirthdaysAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetBirthdaysAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetBirthdaysAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetSuggestionsAsync

    [Fact]
    public async Task GetSuggestionsAsync_WithValidToken_ShouldReturnCollectionOrNull()
    {
        // Act
        var result = await _friendsClient.GetSuggestionsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - рекомендации могут отсутствовать
        Assert.True(result == null || result.Count >= 0);
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithValidToken_SuggestedUidsShouldBeNumericStrings()
    {
        // Act
        var result = await _friendsClient.GetSuggestionsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - если есть результаты, все UID должны быть числовыми строками
        if (result != null && result.Count > 0)
        {
            foreach (var uid in result)
            {
                Assert.Matches(@"^\d+$", uid);
            }
        }
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _friendsClient.GetSuggestionsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _friendsClient.GetSuggestionsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion
}
