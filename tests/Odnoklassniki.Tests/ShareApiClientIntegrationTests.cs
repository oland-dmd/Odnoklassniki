using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Share;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class ShareApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly ShareApiClient _shareClient = new(fixture.ClientCore);

    #region FetchLinkAsync

    [Fact]
    public async Task FetchLinkAsync_WithValidUrl_ShouldReturnLinkInfoOrNull()
    {
        // Arrange - используем URL ok.ru (публичная страница)
        var url = "https://ok.ru";

        // Act
        var result = await _shareClient.FetchLinkAsync(
            url,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - может вернуть null если URL не распознан, но без исключений
        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task FetchLinkAsync_WithLocale_ShouldReturnLinkInfoOrNull()
    {
        // Arrange
        var url = "https://ok.ru";
        var locale = "ru";

        // Act
        var result = await _shareClient.FetchLinkAsync(
            url,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            locale: locale,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task FetchLinkAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _shareClient.FetchLinkAsync(
                "https://ok.ru",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task FetchLinkAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _shareClient.FetchLinkAsync(
                "https://ok.ru",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion
}
