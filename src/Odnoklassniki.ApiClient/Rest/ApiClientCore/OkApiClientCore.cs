using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Odnoklassniki.Exceptions;
using Odnoklassniki.Interfaces;

namespace Odnoklassniki.Rest.ApiClientCore;

/// <summary>
/// Базовый контракт для вызова методов API Одноклассников (OK.ru).
/// Предоставляет унифицированный способ выполнения подписанных запросов
/// к методам OK.ru с поддержкой пользовательской и основной сессии.
/// </summary>
public class OkApiClientCore : IOkApiClientCore, IDisposable
{
    private readonly HttpClient _httpClient = new() 
    {
        BaseAddress = new Uri("https://api.ok.ru/")
    };
    private readonly IOptions<ApplicationOptions> _options;

    public OkApiClientCore(IOptions<ApplicationOptions> options)
    {
        _options = options;

        if (!string.IsNullOrWhiteSpace(_options.Value.UserAgent))
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.Value.UserAgent);
        }
    }

    /// <inheritdoc/>
    public async Task<string?> CallAsync(
        string methodName,
        string accessToken = "",
        string sessionSecretKey = "",
        RestParameters? parameters = null,
        bool? markOnline = false, CancellationToken cancellationToken = default)
    {
        ThrowIfEmpty(methodName, nameof(methodName));
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            accessToken = _options.Value.AccessToken;
        }
        if (string.IsNullOrWhiteSpace(sessionSecretKey))
        {
            sessionSecretKey = _options.Value.SessionSecretKey;
        }
        
        var rootParams = new Dictionary<string, string>
        {
            ["method"] = methodName,
            ["format"] = "json",
            ["application_key"] = _options.Value.ApplicationKey,
            ["access_token"] = accessToken
        };

        var mergedParams = parameters?.MergeParameters(rootParams) ?? rootParams;

        var sig = CalculateSig(sessionSecretKey, mergedParams.AsReadOnly());
        mergedParams["sig"] = sig;

        string queryString = GenerateQueryString(mergedParams.AsReadOnly());

        var requestUri = $"fb.do?{queryString}";

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // OK.ru возвращает ошибки в JSON или plain text
            throw new HttpRequestException(
                $"OK.ru API error {(int)response.StatusCode}: {responseBody}");
        }

        OkApiException.ThrowIfError(responseBody);

        return responseBody.StartsWith("null") ? null : responseBody;

    }

    /// <inheritdoc/>
    public async Task<T?> CallAsync<T>(
        string methodName,
        string accessToken = "",
        string sessionSecretKey = "",
        RestParameters? parameters = null,
        bool? markOnline = false, CancellationToken cancellationToken = default)
    {
        var stringResult = await CallAsync(methodName, accessToken, sessionSecretKey, parameters, markOnline, cancellationToken);

        if (string.IsNullOrWhiteSpace(stringResult))
        {
            return default;
        }
        
        if ((typeof(T).IsClass || typeof(T).IsGenericType) && typeof(T) != typeof(string))
        {
            JsonSerializerOptions jsonOptions = new();
            var result = JsonSerializer.Deserialize<T>(stringResult, jsonOptions);

            return result ?? throw new HttpRequestException($"Error deserialize body: {stringResult}");
        }
        else
        {
            var result = (T)Convert.ChangeType(stringResult, typeof(T));

            return result;
        }

    }

    private static void ThrowIfEmpty(string value, string name)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(name);
        }
    }

    private static string GenerateQueryString(IReadOnlyDictionary<string, string> allParams)
    {
        return string.Join("&", allParams.OrderBy(kvp => kvp.Key, StringComparer.Ordinal).Where(pair => !(string.IsNullOrWhiteSpace(pair.Key) || string.IsNullOrWhiteSpace(pair.Value))).Select(kvp =>
                        $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
    }

    private static string CalculateSig(
        string sessionSecretKey,
        IReadOnlyDictionary<string, string> parameters)
    {
        var sigParams = parameters
            .Where(kvp => kvp.Key != "access_token" && kvp.Key != "sig")
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal);

        var paramString = string.Concat(sigParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var md5String = paramString + sessionSecretKey;
        var inputBytes = Encoding.ASCII.GetBytes(md5String);
        var hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
