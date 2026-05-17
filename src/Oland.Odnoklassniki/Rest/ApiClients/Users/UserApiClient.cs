using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Users.Responses;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Rest.ApiClients.Users;

/// <inheritdoc />
public class UserApiClient(IOkApiClientCore okApi) : IUserApiClient
{
    private const string OkClassName = "users";

    private const string GetLoggedInUserMethodName = $"{OkClassName}.getLoggedInUser";

    /// <inheritdoc />
    public async Task<string?> GetLoggedInUserAsync(IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var result = await okApi.CallAsync<string>(GetLoggedInUserMethodName, context.AccessPair, cancellationToken: cancellationToken);

        return result?.Trim('"');
    }

    private const string GetCurrentUserMethodName = $"{OkClassName}.getCurrentUser";

    /// <inheritdoc />
    public async Task<UserData> GetCurrentUserAsync(IRequestContext context, CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFields("first_name", "last_name", "uid");

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context, nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<UserInfoResponse>(GetCurrentUserMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);

        var result = new UserData
        {
            FirstName = response.FirstName,
            LastName = response.LastName,
            UID = response.UID
        };

        return result;
    }

    private const string GetInfoMethodName = $"{OkClassName}.getInfo";

    /// <inheritdoc />
    public async Task<ICollection<TDto>?> GetInfoAsync<TDto>(
        ICollection<string> uids,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        if (uids.Count == 0)
            return [];

        var parameters = new RestParameters()
            .InsertUserIds(uids)
            .InsertFields(fields?.ToArray());

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<TDto>>(GetInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetInfoByMethodName = $"{OkClassName}.getInfoBy";

    /// <inheritdoc />
    public async Task<TDto?> GetInfoByAsync<TDto>(
        string uid,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto
    {
        var parameters = new RestParameters()
            .InsertUserId(uid)
            .InsertFields(fields?.ToArray());

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<UserGetInfoByResponse<TDto>>(GetInfoByMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.User;
    }

    private const string GetAdditionalInfoMethodName = $"{OkClassName}.getAdditionalInfo";

    /// <inheritdoc />
    public async Task<ICollection<UserAdditionalInfoData>?> GetAdditionalInfoAsync(
        ICollection<string> uids,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (uids.Count == 0)
            return [];

        var parameters = new RestParameters()
            .InsertUserIds(uids);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        var response = await okApi.CallAsync<UserAdditionalInfoResponse>(GetAdditionalInfoMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
        return response?.Users;
    }

    private const string SetStatusMethodName = $"{OkClassName}.setStatus";

    /// <inheritdoc />
    public async Task<bool> SetStatusAsync(
        string? status,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters();

        if (status != null)
        {
            parameters = parameters.InsertCustomParameter("status", status);
        }

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(SetStatusMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string HasAppPermissionMethodName = $"{OkClassName}.hasAppPermission";

    /// <inheritdoc />
    public async Task<bool> HasAppPermissionAsync(
        string permission,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("ext_perm", permission);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(HasAppPermissionMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string IsAppUserMethodName = $"{OkClassName}.isAppUser";

    /// <inheritdoc />
    public async Task<bool> IsAppUserAsync(
        string uid,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertUserId(uid);

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<bool>(IsAppUserMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }

    private const string GetCallsLeftMethodName = $"{OkClassName}.getCallsLeft";

    /// <inheritdoc />
    public async Task<ICollection<MethodCallsLeftData>?> GetCallsLeftAsync(
        ICollection<string> methods,
        IRequestContext context,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertCustomParameter("methods", string.Join(',', methods));

        switch (context)
        {
            case MainAccountRequestContext or ExplicitTokenRequestContext:
                parameters = context.Apply(parameters);
                break;
            default:
                throw new UnexpectedRequestContext(context,
                    nameof(MainAccountRequestContext), nameof(ExplicitTokenRequestContext));
        }

        return await okApi.CallAsync<ICollection<MethodCallsLeftData>>(GetCallsLeftMethodName, context.AccessPair, parameters, cancellationToken: cancellationToken);
    }
}
