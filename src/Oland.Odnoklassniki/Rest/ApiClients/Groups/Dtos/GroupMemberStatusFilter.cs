namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// Фильтр по статусу участников для метода <c>group.getMembers</c>.
/// </summary>
public enum GroupMemberStatusFilter
{
    /// <summary>Обычные участники.</summary>
    ACTIVE,

    /// <summary>Модераторы группы.</summary>
    MODERATOR,

    /// <summary>Администраторы группы.</summary>
    ADMIN,

    /// <summary>Заблокированные участники.</summary>
    BLOCKED
}
