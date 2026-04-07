using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.ApiClients.Auth;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")] // Гарантирует последовательный запуск (из-за лимита 10/мес)
[Trait("Category", "Integration")]
public class AuthApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly AuthApiClient _authClient = new(fixture.ClientCore, Substitute.For<ILogger<AuthApiClient>>());

    [Fact]
    public async Task TouchAccountSessionAsync_WithValidUserToken_ShouldReturnTrue()
    {
        // Act
        var result = await _authClient.TouchAccountSessionAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TouchMainSessionAsync_WithValidAppSession_ShouldReturnTrue()
    {
        // Act
        var options = Substitute.For<IOptions<ApplicationOptions>>();
        options.Value.Returns(new ApplicationOptions()
        {
            AccessToken = TestSettings.AccessPair.AccessToken,
            SessionSecretKey =  TestSettings.AccessPair.SessionSecretKey,
            ApplicationKey =  TestSettings.ApplicationKey
        });
        var result = await _authClient.TouchAccountSessionAsync(new MainAccountRequestContext(options), CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}