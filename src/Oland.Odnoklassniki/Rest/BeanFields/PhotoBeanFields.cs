namespace Oland.Odnoklassniki.Rest.BeanFields;

/// <summary>
/// Константы полей фотографии OK.ru для методов <c>photos.getInfo</c>,
/// <c>photos.getUserPhotos</c>, <c>photos.getUserAlbumPhotos</c> и аналогичных.
/// Используются в параметре <c>fields</c> при запросах к API.
/// </summary>
public static class PhotoBeanFields
{
    /// <summary>Уникальный идентификатор фотографии.</summary>
    public const string Id = "photo.id";

    /// <summary>Текстовое описание (подпись) фотографии.</summary>
    public const string Text = "photo.text";

    /// <summary>URL изображения максимального размера.</summary>
    public const string PicMax = "photo.pic_max";

    /// <summary>URL изображения стандартного размера (640px).</summary>
    public const string Pic640 = "photo.pic640x480";

    /// <summary>URL превью изображения (190px).</summary>
    public const string PicSmall = "photo.pic190x190";

    /// <summary>Идентификатор пользователя — владельца фотографии.</summary>
    public const string UserId = "photo.user_id";

    /// <summary>Идентификатор альбома, в котором находится фотография.</summary>
    public const string AlbumId = "photo.album_id";

    /// <summary>Время создания фотографии в миллисекундах (Unix epoch).</summary>
    public const string CreatedMs = "photo.created_ms";

    /// <summary>Количество лайков у фотографии.</summary>
    public const string LikeCount = "photo.like_count";

    /// <summary>Количество комментариев к фотографии.</summary>
    public const string CommentCount = "photo.comment_count";

    /// <summary>Флаг: поставил ли текущий пользователь лайк.</summary>
    public const string Liked = "photo.liked";
}
