using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Share;

/// <summary>
/// Клиент для работы с методами <c>share.*</c> API Одноклассники (OK.ru).
/// </summary>
public class ShareApiClient(IOkApiClientCore okApi) : IShareApiClient
{
    private const string FetchLinkV2MethodName = "share.fetchLinkV2";

    /// <inheritdoc />
    public async Task<LinkInfoData?> FetchLinkAsync(
        string url,
        IRequestContext context,
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        if (context is not (MainAccountRequestContext or ExplicitTokenRequestContext))
            throw new UnexpectedRequestContext(context,
                nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));

        var parameters = new RestParameters()
            .InsertUrl(url)
            .InsertLocale(locale ?? string.Empty);

        return await okApi.CallAsync<LinkInfoData>(
            FetchLinkV2MethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }
}
