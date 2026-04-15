using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

/// <summary>
/// Специализированный ответ API для получения кратких данных товаров (short products).
/// </summary>
/// <typeparam name="TDto">
/// Тип DTO для элементов краткого товара, наследующий <see cref="BaseOkDto"/>.
/// Обычно содержит минимальный набор полей: <c>id</c>, <c>media_text</c>.
/// </typeparam>
/// <remarks>
/// <para>
/// <b>Назначение:</b> оптимизация трафика при получении списков товаров,
/// где не требуются полные данные (например, для превью в ленте или каталоге).
/// </para>
/// <para>
/// <b>Источник данных:</b> метод API <c>market.getByCatalog</c>, <c>market.getProducts</c>
/// с параметром проекции полей.
/// </para>
/// <para>
/// <b>Особенность:</b> переопределяет свойство <see cref="Products"/> с маппингом
/// на JSON-поле <c>short_products</c>, сохраняя совместимость с базовым классом
/// <see cref="ProductsResponse{TDto}"/>.
/// </para>
/// </remarks>
public record ShortProductsResponse<TDto>() : ProductsResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>
    /// Коллекция кратких данных товаров, соответствующих запросу.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>short_products</c> (переопределяет базовое поле <c>products</c>).
    /// Содержит усечённые представления товаров с минимальным набором полей для отображения в списках.
    /// Тип элементов определяется дженерик-параметром <typeparamref name="TDto"/>.
    /// </remarks>
    [JsonPropertyName("short_products")]
    public new ICollection<TDto> Products { get; init; }
}