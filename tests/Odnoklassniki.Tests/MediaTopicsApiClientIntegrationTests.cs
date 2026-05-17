using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Oland.MediaManager.Application.Builders;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.MediaManager.Application.Services;
using Oland.MediaManager.Application.Validation;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Enums;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Models;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.IntegrationTests;

/// <summary>
/// Вспомогательный DTO для тестов mediatopic.getByIds.
/// </summary>
internal record MediaTopicSimpleDto : BaseOkDto
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

[Collection("Integration")]
[Trait("Category", "Integration")]
public class MediaTopicsApiClientIntegrationTests : IClassFixture<OkApiTestFixture>
{
    private readonly MediaTopicsApiClient _mediaTopicsClient;
    private readonly ExplicitTokenRequestContext _userContext;
    private readonly GroupRequestContext _groupContext;
    private readonly MainGroupRequestContext _mainGroupContext;

    public MediaTopicsApiClientIntegrationTests(OkApiTestFixture fixture)
    {
        IMediaService mediaService = new MediaService(new MediaValidator());
        _mediaTopicsClient = new MediaTopicsApiClient(fixture.ClientCore, mediaService);

        _userContext = new ExplicitTokenRequestContext(TestSettings.AccessPair);
        _groupContext = new GroupRequestContext(TestSettings.AccessPair, TestSettings.GroupId);

        var options = Substitute.For<IOptions<ApplicationOptions>>();
        options.Value.Returns(new ApplicationOptions
        {
            AccessToken = TestSettings.AccessPair.AccessToken,
            SessionSecretKey = TestSettings.AccessPair.SessionSecretKey,
            ApplicationKey = TestSettings.ApplicationKey,
            GroupId = TestSettings.GroupId.Value
        });
        _mainGroupContext = new MainGroupRequestContext(options);
    }

    #region PostAsync

    [Fact(Skip = "Недостаточно прав: PUBLISH_TO_STREAM")]
    public async Task PostAsync_WithTextOnUser_ShouldReturnTopicId()
    {
        // Act - публикуем текстовый пост на странице пользователя
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText($"Integration test post {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
            _userContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(topicId);
        Assert.NotEmpty(topicId);

        // Cleanup - удаляем созданный пост
        if (!string.IsNullOrEmpty(topicId))
        {
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _userContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task PostAsync_WithTextOnGroup_ShouldReturnTopicId()
    {
        // Act - публикуем текстовый пост в группе
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText($"Integration test group post {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.GROUP_THEME },
            _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(topicId);

        // Cleanup
        if (!string.IsNullOrEmpty(topicId))
        {
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _groupContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task PostAsync_WithHiddenPost_ShouldReturnTopicId()
    {
        // Act - публикуем скрытый пост
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText("Hidden integration test post"),
            new MediaTopicPostOptions
            {
                Type = MediaTopicOwnerType.GROUP_THEME,
                HiddenPost = true
            },
            _groupContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(topicId);

        // Cleanup
        if (!string.IsNullOrEmpty(topicId))
        {
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _groupContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task PostAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.PostAsync(
                builder => builder.AddText("Test"),
                new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task PostAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _mediaTopicsClient.PostAsync(
                builder => builder.AddText("Test"),
                new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
                _userContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region EditAsync

    [Fact(Skip = "Недостаточно прав: PUBLISH_TO_STREAM")]
    public async Task EditAsync_WithExistingTopic_ShouldReturnTrue()
    {
        // Arrange - создаём пост для редактирования
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText("Original text for edit test"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
            _userContext,
            CancellationToken.None);

        if (topicId == null)
            return; // Пост не создан — тест неприменим

        try
        {
            // Act
            var result = await _mediaTopicsClient.EditAsync(
                topicId,
                builder => builder.AddText("Edited text from integration test"),
                _userContext,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.True(result);
        }
        finally
        {
            // Cleanup
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _userContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task EditAsync_WithInvalidTopicId_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.EditAsync(
                "INVALID_TOPIC_ID_12345",
                builder => builder.AddText("Test"),
                _userContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task EditAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.EditAsync(
                "some_topic_id",
                builder => builder.AddText("Test"),
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task EditAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _mediaTopicsClient.EditAsync(
                "some_topic_id",
                builder => builder.AddText("Test"),
                _userContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region DeleteTopicAsync

    [Fact(Skip = "Недостаточно прав: PUBLISH_TO_STREAM")]
    public async Task DeleteTopicAsync_WithExistingTopic_ShouldReturnTrue()
    {
        // Arrange - создаём пост для удаления
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText("Post to be deleted in integration test"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
            _userContext,
            CancellationToken.None);

        if (topicId == null)
            return;

        // Act
        var result = await _mediaTopicsClient.DeleteTopicAsync(
            topicId,
            _userContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTopicAsync_WithInvalidTopicId_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.DeleteTopicAsync(
                "INVALID_TOPIC_ID_12345",
                _userContext,
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteTopicAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.DeleteTopicAsync(
                "some_topic_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task DeleteTopicAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _mediaTopicsClient.DeleteTopicAsync(
                "some_topic_id",
                _userContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetByIdsAsync

    [Fact(Skip = "Недостаточно прав: PUBLISH_TO_STREAM")]
    public async Task GetByIdsAsync_WithExistingTopicId_ShouldReturnTopics()
    {
        // Arrange - создаём пост, затем получаем его по ID
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText("Post for GetByIds integration test"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
            _userContext,
            CancellationToken.None);

        if (topicId == null)
            return;

        try
        {
            // Act
            var result = await _mediaTopicsClient.GetByIdsAsync<MediaTopicSimpleDto>(
                [topicId],
                _userContext,
                fields: [MediaTopicBeanFields.Id],
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
        finally
        {
            // Cleanup
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _userContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task GetByIdsAsync_WithEmptyTopicIds_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _mediaTopicsClient.GetByIdsAsync<MediaTopicSimpleDto>(
            [],
            _userContext,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdsAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.GetByIdsAsync<MediaTopicSimpleDto>(
                ["some_topic_id"],
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetByIdsAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _mediaTopicsClient.GetByIdsAsync<MediaTopicSimpleDto>(
                ["some_topic_id"],
                _userContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetRepublishedTopicAsync

    [Fact]
    public async Task GetRepublishedTopicAsync_WithInvalidTopicId_ShouldReturnNullOrThrow()
    {
        // Act - несуществующий топик может вернуть null или бросить исключение
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _mediaTopicsClient.GetRepublishedTopicAsync(
                "INVALID_TOPIC_ID_12345",
                _userContext,
                cancellationToken: CancellationToken.None);
        });

        // Assert - либо нет исключения (вернул null), либо OkApiException
        if (exception != null)
            Assert.IsType<OkApiException>(exception);
    }

    [Fact(Skip = "Недостаточно прав: PUBLISH_TO_STREAM")]
    public async Task GetRepublishedTopicAsync_WithExistingTopic_ShouldReturnNullOrTopicId()
    {
        // Arrange - создаём пост
        var topicId = await _mediaTopicsClient.PostAsync(
            builder => builder.AddText("Post for GetRepublished test"),
            new MediaTopicPostOptions { Type = MediaTopicOwnerType.USER },
            _userContext,
            CancellationToken.None);

        if (topicId == null)
            return;

        try
        {
            // Act - проверяем не является ли пост репостом
            var result = await _mediaTopicsClient.GetRepublishedTopicAsync(
                topicId,
                _userContext,
                cancellationToken: CancellationToken.None);

            // Assert - оригинальный пост не является репостом, результат может быть null
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Cleanup
            await _mediaTopicsClient.DeleteTopicAsync(topicId, _userContext, CancellationToken.None);
        }
    }

    [Fact]
    public async Task GetRepublishedTopicAsync_WithInvalidToken_ShouldThrowOkApiException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            await _mediaTopicsClient.GetRepublishedTopicAsync(
                "some_topic_id",
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                cancellationToken: CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetRepublishedTopicAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _mediaTopicsClient.GetRepublishedTopicAsync(
                "some_topic_id",
                _userContext,
                cancellationToken: cancellationTokenSource.Token);
        });
    }

    #endregion

    #region GetPollVotersNavigator

    [Fact]
    public async Task GetPollVotersNavigator_WithInvalidTopicId_ShouldThrowOkApiExceptionOnMove()
    {
        // Act & Assert - при MoveNextAsync API вернёт ошибку на несуществующий топик
        var exception = await Record.ExceptionAsync(async () =>
        {
            var navigator = _mediaTopicsClient.GetPollVotersNavigator(
                "INVALID_TOPIC_ID_12345",
                answerIndex: 0,
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });

        Assert.NotNull(exception);
        Assert.IsType<OkApiException>(exception);
    }

    [Fact]
    public async Task GetPollVotersNavigator_WithInvalidToken_ShouldThrowOkApiExceptionOnMove()
    {
        // Act & Assert
        await Assert.ThrowsAsync<OkApiException>(async () =>
        {
            var navigator = _mediaTopicsClient.GetPollVotersNavigator(
                "some_topic_id",
                answerIndex: 0,
                new ExplicitTokenRequestContext(TestSettings.AccessPair with { AccessToken = "INVALID_TOKEN_12345" }),
                new AnchorConfiguration { Count = 10 },
                cancellationToken: CancellationToken.None);

            await navigator.MoveNextAsync();
        });
    }

    [Fact]
    public async Task GetPollVotersNavigator_WithCancelledToken_ShouldThrowTaskCanceledException()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var navigator = _mediaTopicsClient.GetPollVotersNavigator(
                "some_topic_id",
                answerIndex: 0,
                _userContext,
                new AnchorConfiguration { Count = 10 },
                cancellationToken: cancellationTokenSource.Token);

            await navigator.MoveNextAsync();
        });
    }

    #endregion
}
