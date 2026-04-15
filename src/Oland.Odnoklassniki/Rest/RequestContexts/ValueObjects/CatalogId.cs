namespace Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

/// <summary>
/// Value-объект, представляющий уникальный идентификатор каталога в маркетплейсе Одноклассников.
/// </summary>
/// <remarks>
/// Обеспечивает типизированное представление строкового идентификатора, исключая использование
/// "магических строк" и повышая безопасность контрактов API-клиентов. Используется в параметрах
/// запросов <see cref="IMarketCatalogsApiClient"/> и <see cref="IMarketProductsApiClient"/>.
/// </remarks>
public class CatalogId
{
    /// <summary>
    /// Строковое значение идентификатора каталога. Только для чтения.
    /// </summary>
    /// <remarks>
    /// Инициализируется через конструктор. Значение не может быть <c>null</c> или пустым.
    /// Формат соответствует внутреннему представлению ID в API Одноклассников (например, числовая строка или UUID).
    /// </remarks>
    public string Value { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="CatalogId"/>.
    /// </summary>
    /// <param name="value">
    /// Строковое значение идентификатора. Обязательное поле, не допускает <c>null</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если параметр <paramref name="value"/> равен <c>null</c>.
    /// </exception>
    public CatalogId(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}