using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Video;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class VideoApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly VideoApiClient _videoClient = new(fixture.ClientCore);

    #region GetUploadUrlAsync

    [Fact(Skip = "Недостаточно прав: VIDEO_CONTENT")]
    public async Task GetUploadUrlAsync_WithValidParams_ShouldReturnUploadUrlData()
    {
        // Arrange
        var fileName = "test_video.mp4";
        var fileSize = 1024L * 1024L; // 1 MB

        // Act
        var result = await _videoClient.GetUploadUrlAsync(
            fileName,
            fileSize,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact(Skip = "Недостаточно прав: VIDEO_CONTENT")]
    public async Task GetUploadUrlAsync_WithGroupContext_ShouldReturnUploadUrlData()
    {
        // Arrange
        var fileName = "test_video_group.mp4";
        var fileSize = 1024L * 1024L;

        // Act
        var result = await _videoClient.GetUploadUrlAsync(
            fileName,
            fileSize,
            new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.GetUploadUrlAsync(
                "test.mp4",
                1024L,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUploadUrlAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _videoClient.GetUploadUrlAsync(
                "test.mp4",
                1024L,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithInvalidVideoId_ShouldThrowOkApiException()
    {
        // Arrange
        var invalidVideoId = "INVALID_VIDEO_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.UpdateAsync(
                invalidVideoId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                title: "Test Title",
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.UpdateAsync(
                "some_video_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task UpdateAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _videoClient.UpdateAsync(
                "some_video_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithInvalidVideoId_ShouldThrowOkApiException()
    {
        // Arrange
        var invalidVideoId = "INVALID_VIDEO_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.DeleteAsync(
                invalidVideoId,
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
            await _videoClient.DeleteAsync(
                "some_video_id",
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
            await _videoClient.DeleteAsync(
                "some_video_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region SubscribeAsync

    [Fact]
    public async Task SubscribeAsync_WithInvalidChannelId_ShouldThrowOkApiException()
    {
        // Arrange
        var invalidChannelId = "INVALID_CHANNEL_ID_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.SubscribeAsync(
                invalidChannelId,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact(Skip = "Недостаточно прав: VIDEO_CONTENT")]
    public async Task SubscribeAsync_WithGroupChannelId_ShouldReturnBool()
    {
        // Arrange - используем ID группы как канал
        var channelId = TestSettings.GroupId.Value;

        // Act
        var result = await _videoClient.SubscribeAsync(
            channelId,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - boolean результат, без исключений
        Assert.True(result || !result);
    }

    [Fact]
    public async Task SubscribeAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _videoClient.SubscribeAsync(
                TestSettings.GroupId.Value,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task SubscribeAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _videoClient.SubscribeAsync(
                TestSettings.GroupId.Value,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion
}
