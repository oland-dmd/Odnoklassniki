using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Stream.Responses;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Stream;

/// <inheritdoc />
public class StreamApiClient(IOkApiClientCore okApi) : IStreamApiClient
{
    private const string OkClassName = "stream";

    private const string DeleteMethodName = $"{OkClassName}.delete";

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        string deleteId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertDeleteId(deleteId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<bool>(DeleteMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response;
    }

    private const string MarkAsSpamMethodName = $"{OkClassName}.markAsSpam";

    /// <inheritdoc />
    public async Task<bool> MarkAsSpamAsync(
        string markAsSpamId,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertMarkAsSpamId(markAsSpamId);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext),
                    nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<bool>(MarkAsSpamMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response;
    }

    private const string IsSubscribedMethodName = $"{OkClassName}.isSubscribed";

    /// <inheritdoc />
    public async Task<bool> IsSubscribedAsync(
        string ownerId,
        bool isGroup,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        // API принимает fid (пользователь) или gid (группа) — взаимоисключающие параметры
        var parameters = new RestParameters();
        if (isGroup)
        {
            parameters = parameters.InsertGroupId(ownerId);
        }
        else
        {
            parameters = parameters.InsertFriendId(ownerId);
        }

        parameters = context switch
        {
            MainAccountRequestContext or ExplicitTokenRequestContext => context.Apply(parameters),
            _ => throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext),
                nameof(ExplicitTokenRequestContext))
        };

        var response = await okApi.CallAsync<IsSubscribedResponse>(IsSubscribedMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.Subscribed ?? false;
    }
}
