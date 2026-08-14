using Oland.Odnoklassniki.Image;

namespace Oland.Odnoklassniki.IntegrationTests;

/// <summary>
/// Юнит-тесты (без реальной сети) — проверяют регрессию #577: ImageClient должен использовать
/// переданный ему HttpClient, а не создавать new HttpClient() внутри каждого метода.
/// </summary>
public class ImageClientUnitTests
{
    [Fact]
    public async Task DownloadAsStreamAsync_UsesInjectedHttpClient()
    {
        var handler = new RecordingHandler { ResponseBody = "картинка" };
        var httpClient = new HttpClient(handler);
        var sut = new ImageClient(httpClient);

        await sut.DownloadAsStreamAsync("https://example.com/img.jpg", CancellationToken.None);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("https://example.com/img.jpg", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task UploadImageAsync_UsesInjectedHttpClient()
    {
        var handler = new RecordingHandler
        {
            ResponseBody = """{"photos":{"pic0":{"token":"tok-1"}}}"""
        };
        var httpClient = new HttpClient(handler);
        var sut = new ImageClient(httpClient);

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await sut.UploadImageAsync("https://upload.ok.ru/put", stream, CancellationToken.None);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("tok-1", result["pic0"]);
    }
}
