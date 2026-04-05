using Microsoft.Extensions.Logging;
using NSubstitute;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class DiscussionApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly DiscussionsApiClient _discussionClient = new(fixture.ClientCore, Substitute.For<ILogger<DiscussionsApiClient>>());

    [Fact]
    public async Task GetGroupListAsync_WithValidUserToken_ShouldReturnValidGroupData()
    {
        // Act
        var result = await _discussionClient.GetGroupListAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGroupListAsync_WithValidUserToken_ShouldReturnValidPersonalDiscussion()
    {
        // Act
        var result = await _discussionClient.GetUserListAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair));
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCommentsAsync_WithValidUserToken_ShouldReturnValidData()
    {
        // Act
        var result = await _discussionClient.GetCommentsAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            TestSettings.DiscussionId,
            "USER_PHOTO");
        
        // Assert
        Assert.NotNull(result);
    }
}