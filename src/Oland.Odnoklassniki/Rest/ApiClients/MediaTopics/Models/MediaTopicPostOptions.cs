using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Enums;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Models;

/// <summary>
/// Параметры публикации медиатопика через <c>mediatopic.post</c>.
/// Задаёт метаданные поста: тип, видимость, дату, ограничения.
/// </summary>
public sealed record MediaTopicPostOptions
{
    /// <summary>
    /// Тип публикации (от чьего имени). По умолчанию — <see cref="MediaTopicOwnerType.USER"/>.
    /// Для группового контекста по умолчанию используется <see cref="MediaTopicOwnerType.GROUP_THEME"/>.
    /// </summary>
    public MediaTopicOwnerType Type { get; init; } = MediaTopicOwnerType.USER;

    /// <summary>
    /// Публиковать ли от имени группы (параметр <c>onBehalfOfGroup</c>).
    /// Имеет смысл только в групповом контексте.
    /// </summary>
    public bool OnBehalfOfGroup { get; init; }

    /// <summary>
    /// Время отложенной публикации (московское время). <see langword="null"/> — опубликовать немедленно.
    /// </summary>
    public DateTime? PublishAt { get; init; }

    /// <summary>Отключить комментарии к посту.</summary>
    public bool DisableComments { get; init; }

    /// <summary>Скрытая публикация (не отображается в ленте подписчиков).</summary>
    public bool HiddenPost { get; init; }

    /// <summary>Показывать предпросмотр ссылки в тексте. По умолчанию <see langword="true"/>.</summary>
    public bool TextLinkPreview { get; init; } = true;

    /// <summary>Установить пост как статус пользователя.</summary>
    public bool SetStatus { get; init; }

    /// <summary>Код локали для локализованных ответов (например, <c>ru</c>, <c>en</c>).</summary>
    public string? Locale { get; init; }
}
