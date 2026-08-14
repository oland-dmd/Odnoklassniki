using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.ApiClientCore;

/// <summary>
/// Базовый контракт для вызова методов API Одноклассников (OK.ru).
/// Предоставляет унифицированный способ выполнения подписанных запросов
/// к методам OK.ru с поддержкой пользовательской и основной сессии.
/// </summary>
public class OkApiClientCore : IOkApiClientCore
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ApplicationOptions> _options;

    /// <summary>
    /// Порог длины query string, выше которого запрос отправляется POST'ом, а не GET'ом, — чтобы не
    /// упереться в лимит длины URL у шлюза OK (при большом attachment он отдаёт HTTP 400). Значение с
    /// запасом ниже типичного лимита строки запроса (~8 КБ), чтобы учесть накладные расходы заголовков.
    /// </summary>
    private const int MaxGetQueryLength = 1800;

    /// <summary>
    /// HttpClient приходит через DI (см. ServiceExtensions.AddOkApiClients — AddHttpClient),
    /// а не создаётся здесь: с AddScoped+new HttpClient() каждый scope плодил новый handler/пул
    /// соединений — риск socket exhaustion под нагрузкой (#577).
    /// </summary>
    public OkApiClientCore(HttpClient httpClient, IOptions<ApplicationOptions> options)
    {
        _httpClient = httpClient;
        _options = options;

        if (!string.IsNullOrWhiteSpace(_options.Value.UserAgent))
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.Value.UserAgent);
        }
    }

    /// <inheritdoc/>
    public async Task<string?> CallAsync(
        string methodName,
        AccessPair accessPair,
        RestParameters? parameters = null,
        bool? markOnline = false, CancellationToken cancellationToken = default)
    {
        ThrowIfEmpty(methodName, nameof(methodName));
        
        var rootParams = new Dictionary<string, string>
        {
            ["method"] = methodName,
            ["format"] = "json",
            ["application_key"] = _options.Value.ApplicationKey,
            ["access_token"] = accessPair.AccessToken
        };

        var mergedParams = parameters?.MergeParameters(rootParams) ?? rootParams;

        var sig = CalculateSig(accessPair.SessionSecretKey, mergedParams.AsReadOnly());
        mergedParams["sig"] = sig;

        var queryString = GenerateQueryString(mergedParams.AsReadOnly());

        // Крупные запросы (market.add/edit, mediatopic.post с большим attachment и т.п.) отправляем
        // POST'ом с телом form-urlencoded. Иначе весь attachment уходит в query string GET-запроса, и при
        // длинном описании/медиа длина URL превышает лимит шлюза OK — он отвечает голым HTTP 400
        // (Tomcat "Bad Request"), а не JSON-ошибкой. Короткие запросы оставляем GET'ом без изменений.
        using var response = queryString.Length > MaxGetQueryLength
            ? await _httpClient.PostAsync("fb.do", new FormUrlEncodedContent(mergedParams), cancellationToken)
            : await _httpClient.GetAsync($"fb.do?{queryString}", cancellationToken);

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
        AccessPair accessPair,
        RestParameters? parameters = null,
        bool? markOnline = false, CancellationToken cancellationToken = default)
    {
        var stringResult = await CallAsync(methodName, accessPair, parameters, markOnline, cancellationToken);

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
}
