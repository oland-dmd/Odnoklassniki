using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Groups.Responses;

internal class UserGroupsResponse
{
    [JsonPropertyName("groups")]
    public required ICollection<GroupResponse>? Response { get; init; }
    
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }
}