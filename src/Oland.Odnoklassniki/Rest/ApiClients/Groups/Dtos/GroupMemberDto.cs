using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO участника группы. Используется в методе <c>group.getMembers</c>.
/// </summary>
public sealed record GroupMemberDto : BaseOkDto
{
    /// <summary>Идентификатор пользователя-участника.</summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    /// <summary>Идентификатор группы.</summary>
    [JsonPropertyName("groupId")]
    public string? GroupId { get; init; }

    /// <summary>Статус участника (например, <c>ACTIVE</c>, <c>MODERATOR</c>, <c>ADMIN</c>).</summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>Роли участника (например, <c>ANALYST</c>, <c>EDITOR</c>, <c>MODERATOR</c>, <c>SUPER_MODERATOR</c>).</summary>
    [JsonPropertyName("role")]
    public ICollection<string>? Role { get; init; }

    /// <summary>Причина блокировки (заполняется только для заблокированных участников).</summary>
    [JsonPropertyName("block_reason")]
    public string? BlockReason { get; init; }

    /// <summary>Дата разблокировки в миллисекундах Unix epoch (заполняется при временной блокировке).</summary>
    [JsonPropertyName("unblock_date_ms")]
    public long? UnblockDateMs { get; init; }
}
