using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.IntegrationTests;

/// <summary>
/// Юнит-тесты (без реальной сети/кредов OK, в отличие от остальных файлов в этом проекте) —
/// проверяют регрессию #577: OkApiClientCore должен использовать переданный ему HttpClient,
/// а не создавать свой на каждый экземпляр (см. AddScoped в ServiceExtensions).
/// </summary>
public class OkApiClientCoreUnitTests
{
    private static ApplicationOptions Options() => new()
    {
        ApplicationKey = "app-key",
        AccessToken = "token",
        SessionSecretKey = "secret",
        GroupId = "0"
    };

    [Fact]
    public async Task CallAsync_SendsRequestThroughInjectedHttpClient()
    {
        var handler = new RecordingHandler { ResponseBody = "null" };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.ok.ru/") };
        var sut = new OkApiClientCore(httpClient, new OptionsWrapper<ApplicationOptions>(Options()));

        await sut.CallAsync("users.getCurrentUser", new AccessPair { AccessToken = "token", SessionSecretKey = "secret" });

        Assert.NotNull(handler.LastRequest);
    }

    [Fact]
    public async Task CallAsync_AppliesUserAgentFromOptions_ToInjectedClient()
    {
        // Раньше UserAgent выставлялся один раз на приватный HttpClient конструктором — переносим
        // проверку, что это по-прежнему работает и на инжектированном клиенте.
        var handler = new RecordingHandler { ResponseBody = "null" };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.ok.ru/") };
        var withUserAgent = new ApplicationOptions
        {
            ApplicationKey = "app-key", AccessToken = "token", SessionSecretKey = "secret", GroupId = "0",
            UserAgent = "FlowersCRM/test"
        };
        var sut = new OkApiClientCore(httpClient, new OptionsWrapper<ApplicationOptions>(withUserAgent));

        await sut.CallAsync("users.getCurrentUser", new AccessPair { AccessToken = "token", SessionSecretKey = "secret" });

        Assert.Equal("FlowersCRM/test", httpClient.DefaultRequestHeaders.UserAgent.ToString());
    }
}
