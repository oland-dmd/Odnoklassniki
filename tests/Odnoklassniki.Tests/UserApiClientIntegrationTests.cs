using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Users;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class UserApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly UserApiClient _userClient = new(fixture.ClientCore);

    #region GetLoggedInUserAsync

    [Fact]
    public async Task GetLoggedInUserAsync_WithValidUserToken_ShouldReturnUserId()
    {
        // Act
        var userId = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(userId);
        Assert.NotEmpty(userId);
        // UID в OK обычно числовой строкой
        Assert.Matches(@"^\d+$", userId);
    }

    [Fact]
    public async Task GetLoggedInUserAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetLoggedInUserAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = invalidToken }),
                cancellationToken: CancellationToken.None);
        });
    }

    #endregion

    #region GetCurrentUser

    [Fact]
    public async Task GetCurrentUser_WithValidUserToken_ShouldReturnValidUserData()
    {
        // Act
        var userData = await _userClient.GetCurrentUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(userData);
        Assert.NotEmpty(userData.UID);
        // Хотя бы одно из полей имени должно быть заполнено
        Assert.True(!string.IsNullOrEmpty(userData.FirstName) || !string.IsNullOrEmpty(userData.LastName));
    }

    [Fact]
    public async Task GetCurrentUser_WithValidUserToken_ShouldReturnConsistentUID()
    {
        // Arrange & Act - получаем UID двумя разными методами
        var loggedInUserId = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        var currentUserData = await _userClient.GetCurrentUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Assert - UID должен совпадать
        Assert.Equal(loggedInUserId, currentUserData.UID);
    }

    [Fact]
    public async Task GetCurrentUser_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetCurrentUserAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = invalidToken }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ShouldReturnUserDataWithExpectedFields()
    {
        // Act
        var userData = await _userClient.GetCurrentUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(userData);
        Assert.NotEmpty(userData.UID);
        // Проверяем, что поля не null (могут быть пустыми строками)
        Assert.NotNull(userData.FirstName);
        Assert.NotNull(userData.LastName);
    }

    #endregion

    #region Cancellation Token

    [Fact]
    public async Task GetLoggedInUserAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetLoggedInUserAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task GetCurrentUser_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetCurrentUserAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion
}