using System.Net;

namespace Oland.Odnoklassniki.IntegrationTests;

/// <summary>
/// Фейковый HttpMessageHandler для юнит-тестов клиентов — доказывает, что класс использует
/// переданный ему HttpClient, а не создаёт свой (регрессия на #577: раньше OkApiClientCore/
/// ImageClient создавали new HttpClient() сами, и подменить транспорт в тесте было невозможно).
/// </summary>
public class RecordingHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }
    public string ResponseBody { get; set; } = "null";
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(new HttpResponseMessage(StatusCode) { Content = new StringContent(ResponseBody) });
    }
}
