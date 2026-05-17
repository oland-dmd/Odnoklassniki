namespace Oland.Odnoklassniki.Rest.BeanFields;

/// <summary>
/// Константы полей обсуждений OK.ru для методов <c>discussions.getList</c>,
/// <c>discussions.get</c>, <c>discussions.getComments</c> и аналогичных.
/// </summary>
public static class DiscussionBeanFields
{
    /// <summary>Уникальный идентификатор обсуждения.</summary>
    public const string Id = "discussion.id";

    /// <summary>Тип обсуждения (GROUP_TOPIC, USER_STATUS и т.п.).</summary>
    public const string Type = "discussion.type";

    /// <summary>Заголовок обсуждения.</summary>
    public const string Title = "discussion.title";

    /// <summary>Текст обсуждения.</summary>
    public const string Text = "discussion.text";

    /// <summary>Дата создания.</summary>
    public const string Date = "discussion.date";

    /// <summary>Идентификатор автора.</summary>
    public const string AuthorId = "discussion.author_id";

    /// <summary>Количество лайков.</summary>
    public const string LikeCount = "discussion.like_count";

    /// <summary>Количество комментариев.</summary>
    public const string CommentCount = "discussion.comment_count";

    /// <summary>Количество репостов.</summary>
    public const string ReshareCount = "discussion.reshare_count";

    /// <summary>Медиа-вложение (attachment JSON).</summary>
    public const string Attachment = "discussion.attachment";

    /// <summary>Идентификатор группы, которой принадлежит обсуждение.</summary>
    public const string GroupId = "discussion.group_id";

    /// <summary>URL обсуждения.</summary>
    public const string Url = "discussion.url";

    /// <summary>Поставил ли текущий пользователь лайк.</summary>
    public const string Liked = "discussion.liked";
}
