namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Enums;

/// <summary>
/// Тип обсуждения OK.ru (параметр <c>types</c> метода <c>discussions.getList</c>).
/// </summary>
public enum ApiDiscussionType
{
    /// <summary>Чат.</summary>
    CHAT,

    /// <summary>Городская новость.</summary>
    CITY_NEWS,

    /// <summary>Статья Дзена.</summary>
    DZEN_ARTICLE,

    /// <summary>Фильм в группе.</summary>
    GROUP_MOVIE,

    /// <summary>Фотография группы.</summary>
    GROUP_PHOTO,

    /// <summary>Товар группы.</summary>
    GROUP_PRODUCT,

    /// <summary>Тема группы (обычный пост).</summary>
    GROUP_TOPIC,

    /// <summary>Событие/мероприятие.</summary>
    HAPPENING_TOPIC,

    /// <summary>Вопрос хобби.</summary>
    HOBBY_QUESTION,

    /// <summary>Фильм.</summary>
    MOVIE,

    /// <summary>Предложение.</summary>
    OFFER,

    /// <summary>Подарок.</summary>
    PRESENT,

    /// <summary>Школьный форум.</summary>
    SCHOOL_FORUM,

    /// <summary>Репост.</summary>
    SHARE,

    /// <summary>Альбом пользователя.</summary>
    USER_ALBUM,

    /// <summary>Форум пользователя.</summary>
    USER_FORUM,

    /// <summary>Фотография пользователя.</summary>
    USER_PHOTO,

    /// <summary>Товар пользователя.</summary>
    USER_PRODUCT,

    /// <summary>Статус пользователя.</summary>
    USER_STATUS,

    /// <summary>VK-клип пользователя.</summary>
    USER_VK_CLIP,

    /// <summary>VK-клип.</summary>
    VK_CLIP
}
