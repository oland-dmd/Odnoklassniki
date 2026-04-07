using Microsoft.Extensions.Options;
using NSubstitute;
using Oland.Odnoklassniki.Enums;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.ApiClients.Groups;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.IntegrationTests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class GroupsApiClientIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly GroupsApiClient _groupsClient;

    public GroupsApiClientIntegrationTests(OkApiTestFixture fixture)
    {
        var options = Substitute.For<IOptions<ApplicationOptions>>();
        options.Value.Returns(new ApplicationOptions()
        {
            AccessToken = TestSettings.AccessPair.AccessToken,
            SessionSecretKey =  TestSettings.AccessPair.SessionSecretKey,
            ApplicationKey =  TestSettings.ApplicationKey
        });
        
        _groupsClient = new GroupsApiClient(fixture.ClientCore, new MainAccountRequestContext(options));
    }

    #region GetInfoAsync (Получение информации о группах)

    [Fact]
    public async Task GetInfoAsync_WithValidGroupIds_ShouldReturnValidGroupInfo()
    {
        // Arrange
        var groupIds = new[] { TestSettings.GroupId.Value };

        // Act
        var result = await _groupsClient.GetGroupsInfoAsync(
            groupIds: groupIds,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(groupIds.Length, result.Count);
    }

    [Fact]
    public async Task GetInfoAsync_WithMultipleValidGroupIds_ShouldReturnAllGroupInfo()
    {
        // Arrange
        var groupIds = new[] { TestSettings.GroupId.Value, TestSettings.GroupId.Value };

        // Act
        var result = await _groupsClient.GetGroupsInfoAsync(
            groupIds: groupIds,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(groupIds.Length, result.Count);
    }

    [Fact]
    public async Task GetInfoAsync_WithInvalidGroupId_ShouldReturnEmptyOrNull()
    {
        // Arrange
        var groupIds = new[] { "INVALID_GROUP_ID_12345" };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _groupsClient.GetGroupsInfoAsync(
                groupIds: groupIds,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetInfoAsync_WithEmptyGroupIds_ShouldReturnEmptyResult()
    {
        // Arrange
        var groupIds = new string[] { };

        // Act & Assert
        var groups = await _groupsClient.GetGroupsInfoAsync(
            groupIds: groupIds,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);
        
        Assert.Empty(groups!);
    }

    [Fact]
    public async Task GetInfoAsync_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";
        var groupIds = new[] { TestSettings.GroupId.Value };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _groupsClient.GetGroupsInfoAsync(
                groupIds: groupIds,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with{AccessToken = invalidToken}),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetInfoAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var groupIds = new[] { TestSettings.GroupId.Value };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _groupsClient.GetGroupsInfoAsync(
                groupIds: groupIds,
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    [Fact]
    public async Task GetInfoAsync_WithValidGroupIds_ShouldReturnGroupWithExpectedFields()
    {
        // Arrange
        var groupIds = new[] { TestSettings.GroupId.Value };

        // Act
        var result = await _groupsClient.GetGroupsInfoAsync(
            groupIds: groupIds,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var groupInfo = result.FirstOrDefault();
        Assert.NotNull(groupInfo);
        Assert.Equal(groupIds[0], groupInfo.Id);
        Assert.NotNull(groupInfo.Name);
    }

    #endregion

    #region GetUserGroupsByIdsAsync (Получение групп пользователя по ID)

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithValidUserId_ShouldReturnValidGroupUserInfo()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var userIds = new List<string> { TestSettings.FriendId.Value };

        // Act
        var result = await _groupsClient.GetUserGroupsInfoByIdsAsync(
            groupId: groupId,
            userIds: userIds,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithMultipleValidUserIds_ShouldReturnAllUserInfo()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var userIds = new List<string> { TestSettings.FriendId.Value, TestSettings.FriendId.Value };

        // Act
        var result = await _groupsClient.GetUserGroupsInfoByIdsAsync(
            groupId: groupId,
            userIds: userIds,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var invalidGroupId = "INVALID_GROUP_ID_12345";
        var userIds = new List<string> { TestSettings.FriendId.Value };

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _groupsClient.GetUserGroupsInfoByIdsAsync(
                groupId: new GroupId(invalidGroupId),
                userIds: userIds,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithEmptyUserIds_ShouldReturnEmptyResult()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var userIds = new List<string> { };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _groupsClient.GetUserGroupsInfoByIdsAsync(
                groupId: groupId,
                userIds: userIds,
                cancellationToken: CancellationToken.None);
        }); 
    }

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithValidUserId_ShouldReturnValidStatus()
    {
        // Arrange
        var groupId = TestSettings.GroupId;
        var userIds = new List<string> { TestSettings.FriendId.Value };

        // Act
        var result = await _groupsClient.GetUserGroupsInfoByIdsAsync(
            groupId: groupId,
            userIds: userIds,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var userInfo = result.FirstOrDefault();
        Assert.NotNull(userInfo);
        Assert.NotNull(userInfo.UserId);
        // Статус должен быть корректным значением enum
        Assert.True(Enum.IsDefined(typeof(GroupStatus), userInfo.Status));
    }

    [Fact]
    public async Task GetUserGroupsByIdsAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var groupId = TestSettings.GroupId;
        var userIds = new List<string> { TestSettings.FriendId.Value };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _groupsClient.GetUserGroupsInfoByIdsAsync(
                groupId: groupId,
                userIds: userIds,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetUserGroupsV2Async (Получение групп пользователя V2)

    [Fact]
    public async Task GetUserGroupsV2Async_WithValidToken_ShouldReturnValidUserGroupData()
    {
        // Arrange
        var anchor = string.Empty;
        var direction = PagingDirection.AROUND;
        var count = 10;

        // Act
        var anchorNavigator = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Direction = direction,
                Count = count,
                Anchor = anchor
            },
            cancellationToken: CancellationToken.None);
        
        await anchorNavigator.MoveNextAsync();
        var firstPage = anchorNavigator.Current;
        
        await anchorNavigator.MoveNextAsync();
        
        var secondPage = anchorNavigator.Current;

        // Assert
        Assert.NotNull(firstPage);
        Assert.NotEmpty(firstPage.Anchor);
        Assert.NotEmpty(firstPage.Results);
        
        Assert.NotNull(secondPage);
        Assert.NotEmpty(secondPage.Anchor);
        Assert.NotEmpty(secondPage.Results);
    }
    
    [Fact]
    public async Task GetUserGroupsV2Async_WithValidToken_ShouldReturnUserDataWithExpectedFields()
    {
        // Arrange
        var anchor = string.Empty;
        var direction = PagingDirection.AROUND;
        var count = 10;

        // Act
        var response = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Direction = direction,
                Count = count,
                Anchor = anchor
            },
            cancellationToken: CancellationToken.None);

        await response.MoveNextAsync();
        var result = response.Current;

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Anchor);
        Assert.NotNull(result.Results);
        Assert.NotEmpty(result.Results);
    }

    [Fact]
    public async Task GetUserGroupsV2Async_WithInvalidToken_ShouldThrowException()
    {
        // Arrange
        var invalidToken = "INVALID_TOKEN_12345";
        var anchor = string.Empty;
        var direction = PagingDirection.AROUND;
        var count = 10;

        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var response = _groupsClient.GetUserGroupsAnchorNavigator(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with {AccessToken = invalidToken}),
                new AnchorConfiguration()
                {
                    Direction = direction,
                    Count = count,
                    Anchor = anchor
                },
                cancellationToken: CancellationToken.None);
            
            await response.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetUserGroupsV2Async_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var anchor = string.Empty;
        var direction = PagingDirection.AROUND;
        var count = 10;

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var response = _groupsClient.GetUserGroupsAnchorNavigator(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                new AnchorConfiguration()
                {
                    Direction = direction,
                    Count = count,
                    Anchor = anchor
                },
                cancellationToken: cancellationTokenSource.Token);

            await response.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetUserGroupsV2Async_WithDifferentPagingDirections_ShouldReturnValidData()
    {
        // Arrange
        var count = 10;

        // Act & Assert - FORWARD
        var navigatorForward = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Count = count,
                Direction = PagingDirection.FORWARD,
                Anchor = string.Empty
            },
            cancellationToken: CancellationToken.None);
         await navigatorForward.MoveNextAsync();
         var resultForward = navigatorForward.Current;
        Assert.NotNull(resultForward);

        // Act & Assert - BACKWARD
        var navigatorBackward = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Direction = PagingDirection.BACKWARD,
                Count = count
            },
            cancellationToken: CancellationToken.None);
        
        await  navigatorBackward.MoveNextAsync();
        var resultBackward = navigatorBackward.Current;
        Assert.NotNull(resultBackward);
    }

    [Fact]
    public async Task GetUserGroupsV2Async_WithDifferentCountValues_ShouldReturnValidData()
    {
        // Arrange
        var anchor = string.Empty;
        var direction = PagingDirection.AROUND;

        // Act & Assert - count = 1
        var navigator1 = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Direction = direction,
                Count = 1,
                Anchor = anchor
            },
            cancellationToken: CancellationToken.None);
        await navigator1.MoveNextAsync();
        var result1 = navigator1.Current;
        Assert.NotNull(result1);
        Assert.Single(result1.Results);

        // Act & Assert - count = 50
        var navigator50 = _groupsClient.GetUserGroupsAnchorNavigator(
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            new AnchorConfiguration()
            {
                Direction = direction,
                Count = 50
            },
            cancellationToken: CancellationToken.None);
        
        await navigator50.MoveNextAsync();
        var result50 = navigator50.Current;
        Assert.NotNull(result50);
        Assert.Equal(50, result50.Results.Count);
    }

    #endregion

    #region Null/Empty Parameter Tests

    [Fact]
    public async Task GetInfoAsync_WithNullAccessToken_ShouldReturnGroupInfo()
    {
        // Arrange
        var groupIds = new[] { TestSettings.GroupId.Value };

        // Act
        var results = await _groupsClient.GetGroupsInfoAsync(
            groupIds: groupIds,
            new ExplicitTokenRequestContext(TestSettings.AccessPair),
            cancellationToken: CancellationToken.None);
        
        Assert.NotNull(results);
        Assert.Equal(groupIds.Length, results.Count);
        Assert.Equal(groupIds[0], results.First().Id);
    }
    #endregion
}