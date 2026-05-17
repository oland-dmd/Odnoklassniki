namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Enums;

/// <summary>
/// Категория обсуждений для фильтрации в методе <c>discussions.getList</c>.
/// Соответствует параметру <c>category</c> типа <c>ApiDiscussionTypeFilter</c>.
/// </summary>
public enum ApiDiscussionTypeFilter
{
    /// <summary>Обсуждения в группах.</summary>
    GROUP,

    /// <summary>Личные обсуждения текущего пользователя.</summary>
    MY
}
