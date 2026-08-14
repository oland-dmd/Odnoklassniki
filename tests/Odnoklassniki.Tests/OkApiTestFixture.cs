// OkApiTestFixture.cs
using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Rest.ApiClientCore;

namespace Oland.Odnoklassniki.IntegrationTests;

public class OkApiTestFixture : IDisposable
{
    private readonly HttpClient? _httpClient;

    public IOkApiClientCore ClientCore { get; }

    public OkApiTestFixture()
    {
        if (!TestSettings.AreCredentialsAvailable)
            return; // Не инициализируем, если креды не заданы

        var options = new ApplicationOptions
        {
            ApplicationKey = TestSettings.ApplicationKey!,
            SessionSecretKey = TestSettings.AccessPair.SessionSecretKey,
            AccessToken = TestSettings.AccessPair.AccessToken,
            GroupId = TestSettings.GroupId.Value
        };

        // OkApiClientCore больше не создаёт HttpClient сам (#577) — фикстура владеет им и чистит в Dispose.
        _httpClient = new HttpClient { BaseAddress = new Uri("https://api.ok.ru/") };
        ClientCore = new OkApiClientCore(_httpClient, new OptionsWrapper<ApplicationOptions>(options));
    }

    public void Dispose() => _httpClient?.Dispose();
}