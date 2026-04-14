namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Constants;

/// <summary>
/// Константы статусов товаров/объявлений в Маркете Одноклассников
/// </summary>
public static class ApiProductStatus
{
    /// <summary>Товар активен и доступен для просмотра или покупки</summary>
    public const string Active = "active";
        
    /// <summary>Товар автоматически закрыт системой (истек срок размещения, лимиты или авто-модерация)</summary>
    public const string AutoClosed = "auto_closed";
        
    /// <summary>Товар закрыт вручную продавцом или администратором</summary>
    public const string Closed = "closed";
        
    /// <summary>Товар временно отсутствует на складе (остаток = 0, но объявление активно)</summary>
    public const string OutOfStock = "out_of_stock";
        
    /// <summary>Товар успешно продан или сделка завершена</summary>
    public const string Sold = "sold";
}