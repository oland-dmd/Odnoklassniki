using System.Net;
using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.ApiClients.Photos;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.AnchorNavigators;

namespace Oland.Odnoklassniki.IntegrationTests;

public sealed class GroupAlbumsListingOfflineTests
{
    private static GroupRequestContext Context() => new(
        new AccessPair { AccessToken = "synthetic", SessionSecretKey = "synthetic" }, new GroupId("123"));

    private static AlbumsApiClient Client(SequenceHandler handler) => new(new OkApiClientCore(
        new HttpClient(handler) { BaseAddress = new Uri("https://synthetic.invalid/") },
        Options.Create(new ApplicationOptions
        { ApplicationKey = "synthetic", AccessToken = "synthetic", SessionSecretKey = "synthetic", GroupId = "123" })));

    [Theory]
    [InlineData("null", "MissingAlbums")]
    [InlineData("{}", "MissingAlbums")]
    [InlineData("{\"albums\":null,\"hasMore\":false}", "MissingAlbums")]
    [InlineData("{\"albums\":[]}", "MissingPagination")]
    [InlineData("{\"albums\":[],\"hasMore\":true,\"pagingAnchor\":\"next\"}", "EmptyContinuation")]
    [InlineData("{\"albums\":[null],\"hasMore\":false}", "InvalidAlbum")]
    public async Task MissingAndIncompleteProtocolCannotProveAbsence(string body, string reason)
    {
        var handler = new SequenceHandler(body);
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal(reason, result.FailureReason);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task ExplicitFinalEmptyPageIsComplete()
    {
        var result = await Client(new("{\"albums\":[],\"hasMore\":false}"))
            .GetCompleteGroupAlbumsAsync(Context());
        Assert.True(result.IsComplete);
        Assert.Empty(result.Albums);
        Assert.Null(result.FailureReason);
    }

    [Fact]
    public async Task TraversalKeepsOwnerAndCapabilityAndDeduplicatesPageBoundary()
    {
        var handler = new SequenceHandler(
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\",\"user_id\":\"9\",\"attrs\":{\"flags\":\"ap\"}}],\"hasMore\":true,\"pagingAnchor\":\"a\"}",
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\",\"user_id\":\"9\",\"attrs\":{\"flags\":\"ap\"}},{\"aid\":\"2\",\"title\":\"Two\"}],\"hasMore\":false}");
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.True(result.IsComplete);
        Assert.Equal(2, result.Albums.Count);
        Assert.Equal("9", result.Albums[0].UserId);
        Assert.True(result.Albums[0].IsAddPhotoAllowed);
        Assert.Null(result.Albums[1].UserId);
        Assert.False(result.Albums[1].IsAddPhotoAllowed);
        Assert.Equal(2, handler.Calls);
        Assert.Contains("pagingAnchor=a", handler.Queries[1]);
        Assert.All(handler.Queries, query =>
        {
            Assert.Contains("gid=123", query);
            Assert.Contains("group_album.user_id", Uri.UnescapeDataString(query));
            Assert.Contains("group_album.ADD_PHOTO_ALLOWED", Uri.UnescapeDataString(query));
            Assert.DoesNotContain("group_album.type", Uri.UnescapeDataString(query));
        });
    }

    [Fact]
    public async Task DuplicateMissingOwnerPreservesIdentityButNotUnconfirmedCapability()
    {
        var result = await Client(new(
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\",\"user_id\":\"9\",\"attrs\":{\"flags\":\"ap\"}}],\"hasMore\":true,\"pagingAnchor\":\"next\"}",
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\"}],\"hasMore\":false}"))
            .GetCompleteGroupAlbumsAsync(Context());
        Assert.True(result.IsComplete);
        Assert.Equal("9", Assert.Single(result.Albums).UserId);
        Assert.False(result.Albums[0].IsAddPhotoAllowed);
    }

    [Fact]
    public async Task DuplicateConflictingOwnersCannotProveInventoryCompleteness()
    {
        var result = await Client(new(
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\",\"user_id\":\"9\"}],\"hasMore\":true,\"pagingAnchor\":\"next\"}",
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\",\"user_id\":\"10\"}],\"hasMore\":false}"))
            .GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("ConflictingOwner", result.FailureReason);
        Assert.Equal("9", Assert.Single(result.Albums).UserId);
    }

    [Fact]
    public async Task LegacyImplementationDefaultsToUnsupportedWithoutCallingOldNavigator()
    {
        IAlbumsApiClient legacy = new LegacyAlbumsClient();
        var result = await legacy.GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("Unsupported", result.FailureReason);
        Assert.Empty(result.Albums);
    }

    [Fact]
    public async Task FiniteReadBudgetNeverPretendsLastPageWasReached()
    {
        var responses = Enumerable.Range(0, 1000).Select(i =>
            $"{{\"albums\":[{{\"aid\":\"{i}\",\"title\":\"One\"}}],\"hasMore\":true,\"pagingAnchor\":\"next{i}\"}}").ToArray();
        var handler = new SequenceHandler(responses);
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("ReadBudgetExceeded", result.FailureReason);
        Assert.Equal(1000, handler.Calls);
        Assert.Equal(1000, result.Albums.Count);
    }

    [Fact]
    public async Task RepeatedAnchorPreservesPositiveEvidenceWithoutCompleteness()
    {
        var body = "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\"}],\"hasMore\":true,\"pagingAnchor\":\"same\"}";
        var handler = new SequenceHandler(body, body);
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("InvalidContinuation", result.FailureReason);
        Assert.Single(result.Albums);
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public async Task ErrorAfterFirstPagePreservesEvidence()
    {
        var handler = new SequenceHandler(
            "{\"albums\":[{\"aid\":\"1\",\"title\":\"One\"}],\"hasMore\":true,\"pagingAnchor\":\"next\"}",
            "{\"error_code\":10,\"error_msg\":\"Synthetic\"}");
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("ApiError", result.FailureReason);
        Assert.Single(result.Albums);
    }

    [Fact]
    public async Task InvalidJsonAndTransportFailureAreNotCompleteEmptyLists()
    {
        var invalid = await Client(new("{" )).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(invalid.IsComplete);
        Assert.Equal("InvalidResponse", invalid.FailureReason);
        var transport = new SequenceHandler("synthetic") { Status = HttpStatusCode.ServiceUnavailable };
        var failed = await Client(transport).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(failed.IsComplete);
        Assert.Equal("TransportError", failed.FailureReason);
    }

    [Fact]
    public async Task CancelledReadThrowsBeforeNetwork()
    {
        var handler = new SequenceHandler("{\"albums\":[],\"hasMore\":false}");
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Client(handler).GetCompleteGroupAlbumsAsync(Context(), cancelled.Token));
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task CancellationDuringReadPropagatesRatherThanReturningVerifiedEmpty()
    {
        using var cancelled = new CancellationTokenSource();
        var handler = new SequenceHandler("{\"albums\":[],\"hasMore\":false}")
        { BeforeResponse = _ => cancelled.Cancel() };
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Client(handler).GetCompleteGroupAlbumsAsync(Context(), cancelled.Token));
    }

    [Fact]
    public async Task HttpTimeoutWithLiveCallerTokenReturnsIncomplete()
    {
        var handler = new SequenceHandler("unused") { Failure = new TaskCanceledException("Synthetic timeout") };
        var result = await Client(handler).GetCompleteGroupAlbumsAsync(Context());
        Assert.False(result.IsComplete);
        Assert.Equal("TransportTimeout", result.FailureReason);
    }

    private sealed class SequenceHandler(params string[] responses) : HttpMessageHandler
    {
        public int Calls { get; private set; }
        public HttpStatusCode Status { get; init; } = HttpStatusCode.OK;
        public Action<CancellationToken>? BeforeResponse { get; init; }
        public Exception? Failure { get; init; }
        public List<string> Queries { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            BeforeResponse?.Invoke(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (Failure is not null)
                return Task.FromException<HttpResponseMessage>(Failure);
            Queries.Add(request.RequestUri!.Query);
            Assert.True(Calls < responses.Length, "Unexpected additional API call");
            var body = responses[Calls++];
            return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(body) });
        }
    }

    private sealed class LegacyAlbumsClient : IAlbumsApiClient
    {
        public Task SetAlbumMainPhotoAsync(string albumId, string photoId, IRequestContext context, CancellationToken cancellationToken = default) => throw new InvalidOperationException();
        public Task<string> CreateAlbumAsync(string title, IRequestContext context, CancellationToken cancellationToken = default) => throw new InvalidOperationException();
        public Task DeleteAlbumAsync(string albumId, IRequestContext context, CancellationToken cancellationToken = default) => throw new InvalidOperationException();
        public Task EditAlbumAsync(string albumId, string title, string description, IRequestContext context, CancellationToken cancellationToken = default, string? type = null) => throw new InvalidOperationException();
        public AnchorNavigator<AlbumData> GetAlbumsNavigatorAsync(IRequestContext context, AnchorConfiguration configuration, CancellationToken cancellationToken = default, params string[] fields) => throw new InvalidOperationException();
        public Task<AlbumData?> GetAlbumInfoAsync(string albumId, IRequestContext context, CancellationToken cancellationToken = default, params string[] fields) => throw new InvalidOperationException();
    }
}
