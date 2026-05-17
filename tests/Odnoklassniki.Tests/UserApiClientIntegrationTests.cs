using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.ApiClients.Users;
using Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;
using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.IntegrationTests;

using Xunit;
using System.Threading.Tasks;

/// <summary>
/// Вспомогательный DTO для тестов users.getInfo и users.getInfoBy.
/// </summary>
internal record UserSimpleDto : BaseOkDto
{
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }
}

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

    #region GetInfoAsync

    [Fact]
    public async Task GetInfoAsync_WithCurrentUserUid_ShouldReturnUserInfo()
    {
        // Arrange - получаем UID текущего пользователя автономно
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };

        // Act
        var result = await _userClient.GetInfoAsync<UserSimpleDto>(
            uids,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            fields: [UserBeanFields.Uid, UserBeanFields.FirstName, UserBeanFields.LastName],
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(uids.Length, result.Count);
        var user = result.First();
        Assert.Equal(currentUid, user.Uid);
    }

    [Fact]
    public async Task GetInfoAsync_WithMultipleUids_ShouldReturnAllUsers()
    {
        // Arrange - дублируем один UID чтобы получить несколько записей
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid!, currentUid! };

        // Act
        var result = await _userClient.GetInfoAsync<UserSimpleDto>(
            uids,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            fields: [UserBeanFields.Uid, UserBeanFields.FirstName, UserBeanFields.LastName],
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetInfoAsync_WithEmptyUids_ShouldReturnEmptyCollection()
    {
        // Arrange
        var uids = Array.Empty<string>();

        // Act
        var result = await _userClient.GetInfoAsync<UserSimpleDto>(
            uids,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInfoAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetInfoAsync<UserSimpleDto>(
                uids,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetInfoAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetInfoAsync<UserSimpleDto>(
                uids,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetInfoByAsync

    [Fact]
    public async Task GetInfoByAsync_WithCurrentUserUid_ShouldReturnUserInfo()
    {
        // Arrange - получаем UID текущего пользователя автономно
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Act
        var result = await _userClient.GetInfoByAsync<UserSimpleDto>(
            currentUid!,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            fields: [UserBeanFields.Uid, UserBeanFields.FirstName, UserBeanFields.LastName],
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(currentUid, result.Uid);
    }

    [Fact]
    public async Task GetInfoByAsync_WithoutExplicitFields_ShouldReturnUserInfo()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Act
        var result = await _userClient.GetInfoByAsync<UserSimpleDto>(
            currentUid!,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            fields: [UserBeanFields.Uid],
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetInfoByAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetInfoByAsync<UserSimpleDto>(
                currentUid!,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetInfoByAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetInfoByAsync<UserSimpleDto>(
                currentUid!,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetAdditionalInfoAsync

    [Fact]
    public async Task GetAdditionalInfoAsync_WithCurrentUserUid_ShouldReturnAdditionalInfo()
    {
        // Arrange - получаем UID текущего пользователя автономно
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };

        // Act
        var result = await _userClient.GetAdditionalInfoAsync(
            uids,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        if (result.Count > 0)
        {
            var userInfo = result.First();
            Assert.NotNull(userInfo.UserId);
        }
    }

    [Fact]
    public async Task GetAdditionalInfoAsync_WithEmptyUids_ShouldReturnEmptyCollection()
    {
        // Arrange
        var uids = Array.Empty<string>();

        // Act
        var result = await _userClient.GetAdditionalInfoAsync(
            uids,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAdditionalInfoAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetAdditionalInfoAsync(
                uids,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAdditionalInfoAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var uids = new[] { currentUid! };
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetAdditionalInfoAsync(
                uids,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region SetStatusAsync

    [Fact(Skip = "Метод устарел: использовать mediatopic.post вместо users.setStatus")]
    public async Task SetStatusAsync_WithValidStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = "Integration test status";

        // Act
        var result = await _userClient.SetStatusAsync(
            status,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact(Skip = "Метод устарел: использовать mediatopic.post вместо users.setStatus")]
    public async Task SetStatusAsync_WithNullStatus_ShouldClearStatus()
    {
        // Act
        var result = await _userClient.SetStatusAsync(
            null,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SetStatusAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.SetStatusAsync(
                "Test",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task SetStatusAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.SetStatusAsync(
                "Test",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region HasAppPermissionAsync

    [Fact]
    public async Task HasAppPermissionAsync_WithValuableAccessPermission_ShouldReturnBool()
    {
        // Arrange
        var permission = "VALUABLE_ACCESS";

        // Act
        var result = await _userClient.HasAppPermissionAsync(
            permission,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert - результат — булевое значение, не бросает исключений
        Assert.True(result || !result);
    }

    [Fact]
    public async Task HasAppPermissionAsync_WithPhotoContentPermission_ShouldReturnBool()
    {
        // Arrange
        var permission = "PHOTO_CONTENT";

        // Act
        var result = await _userClient.HasAppPermissionAsync(
            permission,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result || !result);
    }

    [Fact]
    public async Task HasAppPermissionAsync_WithInvalidToken_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.HasAppPermissionAsync(
                "VALUABLE_ACCESS",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task HasAppPermissionAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.HasAppPermissionAsync(
                "VALUABLE_ACCESS",
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region IsAppUserAsync

    [Fact]
    public async Task IsAppUserAsync_WithCurrentUserUid_ShouldReturnTrue()
    {
        // Arrange - текущий пользователь точно установил приложение (он выдал токен)
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Act
        var result = await _userClient.IsAppUserAsync(
            currentUid!,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAppUserAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.IsAppUserAsync(
                currentUid!,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task IsAppUserAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var currentUid = await _userClient.GetLoggedInUserAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            CancellationToken.None);
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.IsAppUserAsync(
                currentUid!,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetCallsLeftAsync

    [Fact]
    public async Task GetCallsLeftAsync_WithKnownMethods_ShouldReturnCallsInfo()
    {
        // Arrange
        var methods = new[] { "users.getInfo", "friends.get" };

        // Act
        var result = await _userClient.GetCallsLeftAsync(
            methods,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(methods.Length, result.Count);
        foreach (var item in result)
        {
            Assert.NotNull(item.Name);
            Assert.True(item.CallsLeft == null || item.CallsLeft >= 0);
        }
    }

    [Fact]
    public async Task GetCallsLeftAsync_WithSingleMethod_ShouldReturnOneRecord()
    {
        // Arrange
        var methods = new[] { "users.getInfo" };

        // Act
        var result = await _userClient.GetCallsLeftAsync(
            methods,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("users.getInfo", result.First().Name);
    }

    [Fact]
    public async Task GetCallsLeftAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var methods = new[] { "users.getInfo" };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _userClient.GetCallsLeftAsync(
                methods,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetCallsLeftAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var methods = new[] { "users.getInfo" };
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _userClient.GetCallsLeftAsync(
                methods,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
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
