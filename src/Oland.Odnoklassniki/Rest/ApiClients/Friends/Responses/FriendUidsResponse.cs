using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Friends.Responses;

/// <summary>
/// Внутренняя модель ответа API, содержащая список идентификаторов пользователей.
/// Используется в методах <c>friends.getOnline</c>, <c>friends.getAppUsers</c>,
/// <c>friends.getMutualFriends</c>, <c>friends.getByDevices</c>, <c>friends.getSuggestions</c>.
/// </summary>
internal record FriendUidsResponse
{
    /// <summary>Список идентификаторов пользователей.</summary>
    [JsonPropertyName("uids")]
    public ICollection<string>? Uids { get; init; }
}
