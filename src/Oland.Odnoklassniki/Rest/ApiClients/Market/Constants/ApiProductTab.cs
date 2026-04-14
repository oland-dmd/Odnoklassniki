namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Constants;

/// <summary>
/// Константы типов фильтрации и разделов отображения товаров в Маркете Одноклассников
/// </summary>
public class ApiProductTab
{
    /// <summary>Объявления/товары, находящиеся на проверке модерацией перед публикацией</summary>
    public const string OnModeration = "on_moderation";
        
    /// <summary>Товары, созданные или принадлежащие текущему авторизованному пользователю</summary>
    public const string Own = "own";
        
    /// <summary>Опубликованные товары общего каталога (активная витрина)</summary>
    public const string Products = "products";
}