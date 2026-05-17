using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>group.getMembers</c>.
/// </summary>
internal sealed record GroupMembersResponse
{
    /// <summary>Список участников текущей страницы.</summary>
    [JsonPropertyName("members")]
    public ICollection<GroupMemberDto>? Members { get; init; }

    /// <summary>Курсор для перехода к следующей странице.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Признак наличия следующей страницы.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество участников, соответствующих фильтру.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
}
