using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

/// <summary>
/// Ответ API Одноклассников на запрос получения списка товаров с поддержкой курсорной пагинации
/// и расширенными метаданными о согласованности данных.
/// </summary>
/// <typeparam name="TDto">
/// Тип DTO для элементов товара, наследующий <see cref="BaseOkDto"/>.
/// Должен соответствовать структуре данных товара в ответе API.
/// </typeparam>
/// <remarks>
/// <para>
/// <b>Источник данных:</b> методы API <c>market.getByIds</c>, <c>market.getProducts</c>.
/// </para>
/// <para>
/// <b>Пагинация:</b> реализована через курсорный механизм (<c>anchor</c>).
/// Для получения следующей страницы используйте значение <see cref="Anchor"/> из текущего ответа.
/// </para>
/// <para>
/// <b>Кэширование:</b> поле <see cref="Etag"/> поддерживает условные запросы для оптимизации трафика.
/// </para>
/// <para>
/// <b>Согласованность данных:</b> свойство <see cref="Inconsistent"/> указывает на возможные расхождения
/// между кэшированными и актуальными данными (например, при репликации между серверами).
/// </para>
/// </remarks>
public record ProductsResponse<TDto>() where TDto : BaseOkDto
{
    /// <summary>
    /// Курсор для получения следующей страницы результатов.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>anchor</c>.
    /// Пустое значение указывает на отсутствие дополнительных страниц.
    /// </remarks>
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }
    
    /// <summary>
    /// Коллекция товаров, соответствующих запросу.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>products</c>.
    /// Тип элементов определяется дженерик-параметром <typeparamref name="TDto"/>.
    /// Коллекция может быть пустой, если товары не найдены или не соответствуют фильтрам.
    /// </remarks>
    [JsonPropertyName("products")]
    public ICollection<TDto> Products { get; init; }
    
    /// <summary>
    /// ETag-идентификатор версии ответа для механизмов кэширования.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>Etag</c>.
    /// Используется для реализации условных запросов (HTTP <c>If-None-Match</c>).
    /// </remarks>
    [JsonPropertyName("Etag")]
    public string Etag { get; init; }
    
    /// <summary>
    /// Флаг наличия дополнительных страниц результатов.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>has_more</c>.
    /// <c>true</c> — существуют ещё данные по следующему курсору;
    /// <c>false</c> — текущая страница последняя.
    /// </remarks>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
    
    /// <summary>
    /// Общее количество товаров, соответствующих запросу (без учёта пагинации).
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>totalCount</c>.
    /// Значение может быть приблизительным в зависимости от настроек сервера.
    /// </remarks>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
    
    /// <summary>
    /// Флаг возможной несогласованности данных в ответе.
    /// </summary>
    /// <remarks>
    /// <c>true</c> — данные могут быть не полностью актуальными из-за задержек репликации
    /// или кэширования на стороне сервера Одноклассников.
    /// <c>false</c> — данные считаются согласованными.
    /// </remarks>
    public bool Inconsistent { get; init; }
}