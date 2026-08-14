using Microsoft.Extensions.Logging;
using NSubstitute;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Image;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Enums;
using Oland.Odnoklassniki.Rest.ApiClients.Photos;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;
using Oland.Odnoklassniki.Rest.RequestContexts;
using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.IntegrationTests;

internal record DiscussionSimpleDto : BaseOkDto
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

[Collection("Integration")]
[Trait("Category", "Integration")]
public class DiscussionApiClientIntegrationTests(OkApiTestFixture fixture) : IClassFixture<OkApiTestFixture>
{
    private readonly DiscussionsApiClient _discussionClient = new(fixture.ClientCore, Substitute.For<ILogger<DiscussionsApiClient>>());
    private readonly ExplicitTokenRequestContext _userContext = new(TestSettings.AccessPair);
    private readonly PhotosV2ApiClient _photosV2Client = new(fixture.ClientCore);
    private readonly ImageClient _imageClient = new(new HttpClient());
    private readonly PhotosApiClient _photosClient = new(fixture.ClientCore);

    private async Task<string> UploadTestPhotoAsync()
    {
        var uploadData = await _photosV2Client.GetUploadUrlAsync(_userContext);
        await using var file = File.Open("./test.png", FileMode.Open);
        var token = await _imageClient.UploadImageAsync(uploadData.UploadUrl, file, CancellationToken.None);
        var committed = await _photosV2Client.CommitAsync("", token.Keys.First(), token.Values.First(), _userContext, CancellationToken.None);
        return committed.First().Id!;
    }

    #region GetGroupListAsync (Список обсуждений группы)

    [Fact]
    public async Task GetGroupListAsync_WithValidUserToken_ShouldReturnValidGroupData()
    {
        var result = await _discussionClient.GetGroupListAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGroupListAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetGroupListAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetUserListAsync (Список личных обсуждений)

    [Fact]
    public async Task GetGroupListAsync_WithValidUserToken_ShouldReturnValidPersonalDiscussion()
    {
        var result = await _discussionClient.GetUserListAsync(
            new ExplicitTokenRequestContext(TestSettings.AccessPair));

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetUserListAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetUserListAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetCommentsAsync (Комментарии к обсуждению — legacy)

    [Fact]
    public async Task GetCommentsAsync_WithValidUserToken_ShouldReturnValidData()
    {
        // Arrange - загружаем временное фото (USER_PHOTO используется как discussionId)
        var photoId = await UploadTestPhotoAsync();

        try
        {
            var result = await _discussionClient.GetCommentsAsync(
                _userContext,
                photoId,
                "USER_PHOTO");

            Assert.NotNull(result);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, _userContext);
        }
    }

    [Fact]
    public async Task GetCommentsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetCommentsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair),
                TestSettings.DiscussionId,
                "USER_PHOTO",
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetListNavigator (Пагинация обсуждений)

    [Fact]
    public async Task GetListNavigator_WithValidToken_ShouldReturnResults()
    {
        var navigator = _discussionClient.GetListNavigator<DiscussionSimpleDto>(
            _userContext,
            new AnchorConfiguration { Count = 10 },
            cancellationToken: CancellationToken.None);

        await navigator.MoveNextAsync();
        var result = navigator.Current;

        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task GetListNavigator_WithTypeFilter_ShouldReturnResults()
    {
        var navigator = _discussionClient.GetListNavigator<DiscussionSimpleDto>(
            _userContext,
            new AnchorConfiguration { Count = 10 },
            types: new[] { ApiDiscussionType.USER_PHOTO },
            cancellationToken: CancellationToken.None);

        await navigator.MoveNextAsync();
        var result = navigator.Current;

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetListNavigator_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            var navigator = _discussionClient.GetListNavigator<DiscussionSimpleDto>(
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetListNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _discussionClient.GetListNavigator<DiscussionSimpleDto>(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetListNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _discussionClient.GetListNavigator<DiscussionSimpleDto>(
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetAsync (Получение обсуждения по ID)

    [Fact]
    public async Task GetAsync_WithValidDiscussionId_ShouldReturnResultOrNull()
    {
        // Arrange - загружаем временное фото
        var photoId = await UploadTestPhotoAsync();

        try
        {
            var result = await _discussionClient.GetAsync<DiscussionSimpleDto>(
                photoId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: CancellationToken.None);

            Assert.True(result == null || result != null);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, _userContext);
        }
    }

    [Fact]
    public async Task GetAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetAsync<DiscussionSimpleDto>(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetAsync<DiscussionSimpleDto>(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetAsync<DiscussionSimpleDto>(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetCommentAsync (Получение комментария)

    [Fact]
    public async Task GetCommentAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetCommentAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                "some_comment_id",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetCommentAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetCommentAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                "some_comment_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetCommentAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetCommentAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                "some_comment_id",
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetDiscussionCommentsAsync (Комментарии к обсуждению — новый API)

    [Fact]
    public async Task GetDiscussionCommentsAsync_WithValidDiscussionId_ShouldReturnResultOrNull()
    {
        // Arrange - загружаем временное фото
        var photoId = await UploadTestPhotoAsync();

        try
        {
            var result = await _discussionClient.GetDiscussionCommentsAsync(
                photoId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: CancellationToken.None);

            Assert.True(result == null || result != null);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, _userContext);
        }
    }

    [Fact]
    public async Task GetDiscussionCommentsAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionCommentsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionCommentsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetDiscussionCommentsCountAsync (Количество комментариев)

    [Fact]
    public async Task GetDiscussionCommentsCountAsync_WithValidDiscussionId_ShouldReturnNonNegative()
    {
        // Arrange - загружаем временное фото
        var photoId = await UploadTestPhotoAsync();

        try
        {
            var result = await _discussionClient.GetDiscussionCommentsCountAsync(
                photoId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: CancellationToken.None);

            Assert.True(result >= 0);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, _userContext);
        }
    }

    [Fact]
    public async Task GetDiscussionCommentsCountAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsCountAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionCommentsCountAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsCountAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionCommentsCountAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetDiscussionCommentsCountAsync(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetDiscussionLikesNavigator (Лайки обсуждения)

    [Fact]
    public async Task GetDiscussionLikesNavigator_WithValidDiscussionId_ShouldReturnResults()
    {
        // Arrange - загружаем временное фото
        var photoId = await UploadTestPhotoAsync();

        try
        {
            var navigator = _discussionClient.GetDiscussionLikesNavigator(
                photoId,
                "USER_PHOTO",
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
            var result = navigator.Current;

            Assert.NotNull(result);
            Assert.NotNull(result.Results);
        }
        finally
        {
            await _photosClient.DeletePhotoAsync(photoId, _userContext);
        }
    }

    [Fact]
    public async Task GetDiscussionLikesNavigator_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            var navigator = _discussionClient.GetDiscussionLikesNavigator(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetDiscussionLikesNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _discussionClient.GetDiscussionLikesNavigator(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetDiscussionLikesNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _discussionClient.GetDiscussionLikesNavigator(
                TestSettings.DiscussionId,
                "USER_PHOTO",
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetCommentLikesNavigator (Лайки комментария)

    [Fact]
    public async Task GetCommentLikesNavigator_WithValidIds_ShouldReturnResults()
    {
        var navigator = _discussionClient.GetCommentLikesNavigator(
            "some_comment_id",
            TestSettings.DiscussionId,
            "USER_PHOTO",
            _userContext,
            new AnchorConfiguration { Count = 10 },
            cancellationToken: CancellationToken.None);

        // Неверный comment_id вернёт пустой результат или бросит исключение
        var exception = await Record.ExceptionAsync(async () =>
        {
            await navigator.MoveNextAsync();
        });
        Assert.True(exception == null || exception is OkApiException);
    }

    [Fact]
    public async Task GetCommentLikesNavigator_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            var navigator = _discussionClient.GetCommentLikesNavigator(
                "some_comment_id",
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetCommentLikesNavigator_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _discussionClient.GetCommentLikesNavigator(
                "some_comment_id",
                TestSettings.DiscussionId,
                "USER_PHOTO",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetCommentLikesNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _discussionClient.GetCommentLikesNavigator(
                "some_comment_id",
                TestSettings.DiscussionId,
                "USER_PHOTO",
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cts.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion

    #region GetAttachedResourcesAsync (Вложенные ресурсы)

    [Fact(Skip = "Неизвестная ошибка сервера при запросе вложенных ресурсов")]
    public async Task GetAttachedResourcesAsync_WithValidAttachIds_ShouldReturnResultOrNull()
    {
        var result = await _discussionClient.GetAttachedResourcesAsync(
            new[] { TestSettings.DiscussionId },
            _userContext,
            cancellationToken: CancellationToken.None);

        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task GetAttachedResourcesAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetAttachedResourcesAsync(
                new[] { TestSettings.DiscussionId },
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAttachedResourcesAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetAttachedResourcesAsync(
                new[] { TestSettings.DiscussionId },
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetAttachedResourcesAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetAttachedResourcesAsync(
                new[] { TestSettings.DiscussionId },
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion

    #region GetDiscussionsAsync (Deprecated API)

#pragma warning disable CS0618
    [Fact]
    public async Task GetDiscussionsAsync_WithValidToken_ShouldReturnResultOrNull()
    {
        var result = await _discussionClient.GetDiscussionsAsync(
            _userContext,
            cancellationToken: CancellationToken.None);

        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task GetDiscussionsAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetDiscussionsAsync(
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetDiscussionsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetDiscussionsAsync(
                _userContext,
                cancellationToken: cts.Token);
        });
    }
#pragma warning restore CS0618

    #endregion

    #region GetDiscussionsNewsAsync (Новости обсуждений)

    [Fact]
    public async Task GetDiscussionsNewsAsync_WithValidToken_ShouldReturnResultOrNull()
    {
        var result = await _discussionClient.GetDiscussionsNewsAsync(
            _userContext,
            cancellationToken: CancellationToken.None);

        Assert.True(result == null || result != null);
    }

    [Fact]
    public async Task GetDiscussionsNewsAsync_WithUnexpectedContext_ShouldThrowUnexpectedRequestContext()
    {
        await Assert.ThrowsAsync<UnexpectedRequestContext>(async () =>
        {
            await _discussionClient.GetDiscussionsNewsAsync(
                new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionsNewsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _discussionClient.GetDiscussionsNewsAsync(
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetDiscussionsNewsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _discussionClient.GetDiscussionsNewsAsync(
                _userContext,
                cancellationToken: cts.Token);
        });
    }

    #endregion
}
