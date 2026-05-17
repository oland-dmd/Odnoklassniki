using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;

/// <summary>
/// Информация об оставшихся вызовах для одного метода API.
/// Возвращается методом <c>users.getCallsLeft</c>.
/// </summary>
public record MethodCallsLeftData
{
    /// <summary>Название метода API.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Количество оставшихся вызовов.</summary>
    [JsonPropertyName("callsLeft")]
    public int CallsLeft { get; init; }

    /// <summary>Флаг наличия доступных вызовов (<c>true</c> — лимит не исчерпан).</summary>
    [JsonPropertyName("available")]
    public bool Available { get; init; }
}
