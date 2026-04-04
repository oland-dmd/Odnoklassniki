// OkApiTestFixture.cs
using Microsoft.Extensions.Options;
using Odnoklassniki.Interfaces;
using Odnoklassniki.Rest.ApiClientCore;

namespace Odnoklassniki.ApiClient.IntegrationTests;

public class OkApiTestFixture : IDisposable
{
    public IOkApiClientCore ClientCore { get; }

    public OkApiTestFixture()
    {
        if (!TestSettings.AreCredentialsAvailable)
            return; // Не инициализируем, если креды не заданы

        var options = new ApplicationOptions
        {
            ApplicationKey = TestSettings.ApplicationKey!,
            SessionSecretKey = TestSettings.AccessPair.SessionSecretKey,
            AccessToken = TestSettings.AccessPair.AccessToken
        };

        ClientCore = new OkApiClientCore(new OptionsWrapper<ApplicationOptions>(options));
    }

    public void Dispose()
    {
        if (ClientCore is IDisposable disposable)
            disposable.Dispose();
    }
}