using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

/// <summary>
/// Ответ API Одноклассников на запрос получения списка каталогов с поддержкой курсорной пагинации.
/// </summary>
/// <typeparam name="TCatalogDto">
/// Тип DTO для элементов каталога, наследующий <see cref="BaseOkDto"/>.
/// Должен соответствовать структуре данных каталога в ответе API.
/// </typeparam>
/// <remarks>
/// <para>
/// <b>Источник данных:</b> методы API <c>market.getCatalogsByGroup</c>, <c>market.getCatalogsByIds</c>.
/// </para>
/// <para>
/// <b>Пагинация:</b> реализована через курсорный механизм (<c>anchor</c>).
/// Для получения следующей страницы используйте значение <see cref="Anchor"/> из текущего ответа
/// в параметре <c>anchor</c> следующего запроса.
/// </para>
/// <para>
/// <b>Кэширование:</b> поле <see cref="Etag"/> может использоваться для условных запросов
/// (HTTP-заголовок <c>If-None-Match</c>) с целью оптимизации трафика.
/// </para>
/// </remarks>
public record CatalogsResponse<TCatalogDto>() where TCatalogDto : BaseOkDto
{
    /// <summary>
    /// Курсор для получения следующей страницы результатов.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>anchor</c>.
    /// Пустое значение указывает на отсутствие дополнительных страниц.
    /// Используется в параметре <c>anchor</c> при запросе следующей порции данных.
    /// </remarks>
    [JsonPropertyName("anchor")]
    public string Anchor { get; init; }
    
    /// <summary>
    /// Коллекция каталогов, соответствующих запросу.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>catalogs</c>.
    /// Тип элементов определяется дженерик-параметром <typeparamref name="TCatalogDto"/>.
    /// Коллекция может быть пустой, если каталоги не найдены или не соответствуют фильтрам.
    /// </remarks>
    [JsonPropertyName("catalogs")]
    public ICollection<TCatalogDto> Catalogs { get; init; }
    
    /// <summary>
    /// ETag-идентификатор версии ответа для механизмов кэширования.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>Etag</c>.
    /// Может использоваться клиентом для реализации условных запросов:
    /// если данные не изменились, сервер вернёт статус <c>304 Not Modified</c>.
    /// </remarks>
    [JsonPropertyName("Etag")]
    public string Etag { get; init; }
    
    /// <summary>
    /// Флаг наличия дополнительных страниц результатов.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>has_more</c>.
    /// <c>true</c> — существуют ещё данные, доступные по следующему курсору;
    /// <c>false</c> — текущая страница является последней.
    /// </remarks>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
    
    /// <summary>
    /// Общее количество каталогов, соответствующих запросу (без учёта пагинации).
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>totalCount</c>.
    /// Значение может быть приблизительным в зависимости от настроек сервера.
    /// Используется для отображения прогресса пагинации в интерфейсе.
    /// </remarks>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }
};