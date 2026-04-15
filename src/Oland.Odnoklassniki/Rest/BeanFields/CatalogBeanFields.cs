namespace Oland.Odnoklassniki.Rest.BeanFields;

public class CatalogBeanFields
{
    /// <summary>Доступные действия текущего пользователя с элементом каталога (например, edit, delete, publish)</summary>
    public const string Capabilities = "capabilities";
        
    /// <summary>Идентификатор сообщества-владельца каталога</summary>
    public const string GroupId = "group_id";
        
    /// <summary>Является ли изображение основным фото каталога или товара</summary>
    public const string GroupPhoto = "group_photo";
        
    /// <summary>Уникальный идентификатор элемента каталога (товара/позиции)</summary>
    public const string Id = "id";
        
    /// <summary>Базовый URL для формирования ссылок на изображения товара в разных разрешениях</summary>
    public const string ImageUrlBase = "image_url_base";
        
    /// <summary>Название товара или каталога</summary>
    public const string Name = "name";
        
    /// <summary>Разрешение изображения (например, "1200x800") или идентификатор размерной вариации</summary>
    public const string Size = "size";
        
    /// <summary>Идентификатор пользователя, создавшего или загрузившего элемент каталога</summary>
    public const string UserId = "user_id";


}