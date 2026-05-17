using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Friends.Responses;

/// <summary>
/// Внутренняя модель ответа API на запрос <c>friends.getSuggestions</c>.
/// </summary>
internal record FriendSuggestionsResponse
{
    /// <summary>Список рекомендованных пользователей.</summary>
    [JsonPropertyName("users")]
    public ICollection<FriendSuggestionItem>? Users { get; init; }

    /// <summary>Курсор для следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Флаг наличия дополнительных результатов.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
}

/// <summary>
/// Один элемент из ответа <c>friends.getSuggestions</c>.
/// </summary>
internal record FriendSuggestionItem
{
    /// <summary>Идентификатор пользователя.</summary>
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    /// <summary>Количество общих друзей.</summary>
    [JsonPropertyName("common")]
    public int Common { get; init; }
}
