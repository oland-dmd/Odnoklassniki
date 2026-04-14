// OkApiTestFixture.cs
using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Rest.ApiClientCore;

namespace Oland.Odnoklassniki.IntegrationTests;

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
            AccessToken = TestSettings.AccessPair.AccessToken,
            GroupId = TestSettings.GroupId.Value
        };

        ClientCore = new OkApiClientCore(new OptionsWrapper<ApplicationOptions>(options));
    }

    public void Dispose()
    {
        if (ClientCore is IDisposable disposable)
            disposable.Dispose();
    }
}