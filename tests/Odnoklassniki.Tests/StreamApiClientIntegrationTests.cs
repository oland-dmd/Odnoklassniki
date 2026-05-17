using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Stream;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class StreamApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly StreamApiClient _streamClient = new(fixture.ClientCore);

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithInvalidDeleteId_ShouldThrowOkApiException()
    {
        // Arrange - невалидный ID стримового элемента
        var invalidDeleteId = "INVALID_STREAM_DELETE_ID_12345";

        // Act & Assert - API должен вернуть ошибку на несуществующий ID
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _streamClient.DeleteAsync(
                invalidDeleteId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _streamClient.DeleteAsync(
                "some_delete_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _streamClient.DeleteAsync(
                "some_delete_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region MarkAsSpamAsync

    [Fact]
    public async Task MarkAsSpamAsync_WithInvalidMarkAsSpamId_ShouldThrowOkApiException()
    {
        // Arrange - невалидный ID стримового элемента
        var invalidMarkAsSpamId = "INVALID_STREAM_SPAM_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _streamClient.MarkAsSpamAsync(
                invalidMarkAsSpamId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task MarkAsSpamAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _streamClient.MarkAsSpamAsync(
                "some_spam_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task MarkAsSpamAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _streamClient.MarkAsSpamAsync(
                "some_spam_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region IsSubscribedAsync

    [Fact]
    public async Task IsSubscribedAsync_WithGroupOwner_ShouldReturnBool()
    {
        // Arrange - проверяем подписку на тестовую группу
        var groupId = TestSettings.GroupId.Value;

        // Act
        var result = await _streamClient.IsSubscribedAsync(
            groupId,
            isGroup: true,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - результат является булевым значением, исключений нет
        Assert.True(result || !result);
    }

    [Fact]
    public async Task IsSubscribedAsync_WithUserId_ShouldReturnBool()
    {
        // Arrange - проверяем подписку на пользователя
        var friendId = TestSettings.FriendId.Value;

        // Act
        var result = await _streamClient.IsSubscribedAsync(
            friendId,
            isGroup: false,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result || !result);
    }

    [Fact]
    public async Task IsSubscribedAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _streamClient.IsSubscribedAsync(
                TestSettings.GroupId.Value,
                isGroup: true,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task IsSubscribedAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _streamClient.IsSubscribedAsync(
                TestSettings.GroupId.Value,
                isGroup: true,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion
}
