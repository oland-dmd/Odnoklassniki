namespace Oland.Odnoklassniki.Rest.BeanFields;


/// <summary>
/// Константы полей для товаров и рекламных объявлений в Маркете Одноклассников
/// </summary>
public class ShortProductBeanFields
{
    /// <summary>Разрешено ли принятие или отклонение объявления (например, для модераторов или владельцев магазина)</summary>
    public const string AcceptRejectAllowed = "accept_reject_allowed";

    /// <summary>Реферер или идентификатор автора/создателя объявления</summary>
    public const string AuthorRef = "author_ref";

    /// <summary>Внутренний идентификатор, используемый в операциях удаления</summary>
    public const string DeleteId = "delete_id";

    /// <summary>Информация о доступных способах доставки товара</summary>
    public const string Delivery = "delivery";

    /// <summary>Описание товара с поддержкой медиа-разметки или форматированного текста</summary>
    public const string DescriptionMediaText = "description_media_text";

    /// <summary>Флаг или разрешение на редактирование объявления</summary>
    public const string Edit = "edit";

    /// <summary>Статус редактирования или причина временной блокировки изменений</summary>
    public const string EditStatus = "edit_status";

    /// <summary>Основное изображение товара или фото сообщества в карточке объявления</summary>
    public const string GroupPhoto = "group_photo";

    /// <summary>Уникальный идентификатор товара или рекламного объявления</summary>
    public const string Id = "id";

    /// <summary>Базовый URL для формирования путей к изображениям товара в разных разрешениях</summary>
    public const string ImageUrlBase = "image_url_base";

    /// <summary>Флаг, указывающий, что объявление относится к услугам, а не к физическим товарам</summary>
    public const string IsAdForService = "is_ad_for_service";

    /// <summary>Флаг возможности онлайн-оплаты или покупки напрямую через Маркет</summary>
    public const string IsAdSoldOnline = "is_ad_sold_online";

    /// <summary>Идентификатор или ссылка для отправки жалобы/пометки объявления как спама</summary>
    public const string MarkAsSpamId = "mark_as_spam_id";

    /// <summary>Флаг, указывающий, что объявление находится на модерации</summary>
    public const string OnModeration = "on_moderation";

    /// <summary>Закреплено ли объявление в верхней части каталога или витрины</summary>
    public const string Pinned = "pinned";

    /// <summary>Разрешено ли закрепление объявления в каталоге</summary>
    public const string PinAllowed = "pin_allowed";

    /// <summary>Стоимость товара (часто возвращается как объект с полями currency, amount, old_price)</summary>
    public const string Price = "price";

    /// <summary>Реферальный параметр или метка источника перехода к объявлению</summary>
    public const string Ref = "ref";

    /// <summary>Текущий статус объявления (active, paused, archived, rejected, moderation и т.д.)</summary>
    public const string Status = "status";

    /// <summary>Заголовок или название товара/объявления</summary>
    public const string Title = "title";

    /// <summary>Заголовок с поддержкой медиа-форматирования или специальных символов</summary>
    public const string TitleMediaText = "title_media_text";

    /// <summary>Разрешено ли написание сообщений продавцу/автору через карточку товара</summary>
    public const string WriteMessage = "write_message";
}