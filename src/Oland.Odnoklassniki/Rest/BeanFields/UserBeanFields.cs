namespace Oland.Odnoklassniki.Rest.BeanFields;

/// <summary>
/// Константы полей пользователя OK.ru для методов <c>users.getInfo</c>, <c>users.getInfoBy</c>,
/// <c>users.getCurrentUser</c> и аналогичных.
/// </summary>
public static class UserBeanFields
{
    /// <summary>Уникальный идентификатор пользователя.</summary>
    public const string Uid = "uid";

    /// <summary>Имя пользователя.</summary>
    public const string FirstName = "first_name";

    /// <summary>Фамилия пользователя.</summary>
    public const string LastName = "last_name";

    /// <summary>Имя и фамилия (полное имя).</summary>
    public const string Name = "name";

    /// <summary>URL аватара пользователя (малый размер).</summary>
    public const string PicSmall = "pic_small";

    /// <summary>URL аватара пользователя (средний размер).</summary>
    public const string Pic = "pic";

    /// <summary>URL аватара пользователя (большой размер).</summary>
    public const string PicBig = "pic_big";

    /// <summary>URL аватара пользователя (размер 190x190).</summary>
    public const string Pic190 = "pic190";

    /// <summary>Пол пользователя.</summary>
    public const string Gender = "gender";

    /// <summary>Дата рождения (строка).</summary>
    public const string Birthday = "birthday";

    /// <summary>Возраст пользователя.</summary>
    public const string Age = "age";

    /// <summary>Статус пользователя.</summary>
    public const string Status = "status";

    /// <summary>Местонахождение (город, страна).</summary>
    public const string Location = "location";

    /// <summary>Страна проживания.</summary>
    public const string LocationCountry = "location_of_birth";

    /// <summary>Флаг: является ли пользователь другом.</summary>
    public const string Friend = "friend";

    /// <summary>Флаг: заблокирован ли пользователь.</summary>
    public const string Blocked = "blocked";

    /// <summary>Количество друзей.</summary>
    public const string FriendsCount = "friend_count";

    /// <summary>Количество подписчиков.</summary>
    public const string SubscriberCount = "subscriber_count";

    /// <summary>Онлайн-статус.</summary>
    public const string Online = "online";

    /// <summary>Время последнего посещения.</summary>
    public const string LastOnline = "last_online";

    /// <summary>URL профиля пользователя.</summary>
    public const string Url = "url";

    /// <summary>Профессия / должность.</summary>
    public const string Occupation = "occupation";
}
