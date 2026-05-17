namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Enums;

/// <summary>
/// Тип владельца медиатопика OK.ru (параметр <c>type</c> метода <c>mediatopic.post</c>).
/// Определяет, от чьего имени публикуется запись.
/// </summary>
public enum MediaTopicOwnerType
{
    /// <summary>Запись на странице пользователя (обычный пост).</summary>
    USER,

    /// <summary>Статус пользователя.</summary>
    USER_STATUS,

    /// <summary>Заметка на странице пользователя.</summary>
    USER_NOTE,

    /// <summary>Тема в группе (стандартная публикация в группе).</summary>
    GROUP_THEME,

    /// <summary>Промо-тема в группе.</summary>
    GROUP_THEME_PROMO,

    /// <summary>Предложение участника группы (публикуется на модерацию).</summary>
    GROUP_SUGGESTED
}
