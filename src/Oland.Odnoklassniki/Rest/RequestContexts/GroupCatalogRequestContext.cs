using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

namespace Oland.Odnoklassniki.Rest.RequestContexts;

/// <summary>
/// Контекст выполнения запроса, специализированный для работы с конкретными каталогами в группе.
/// </summary>
/// <remarks>
/// <para>
/// Наследует <see cref="GroupRequestContext"/> и добавляет привязку к одному или нескольким каталогам
/// через коллекцию <see cref="CatalogIds"/>.
/// </para>
/// <para>
/// <b>Область применения:</b> методы API, требующие явного указания целевых каталогов
/// (например, <c>market.getByCatalog</c>, <c>market.pin</c>, <c>market.reorder</c>).
/// </para>
/// <para>
/// <b>Неизменяемость:</b> тип объявлен как <c>record</c>, все поля инициализируются в конструкторе
/// и доступны только для чтения.
/// </para>
/// </remarks>
public record GroupCatalogsRequestContext : GroupRequestContext
{
    /// <summary>
    /// Коллекция идентификаторов каталогов, в контексте которых выполняется запрос.
    /// </summary>
    /// <remarks>
    /// Заполняется исключительно через конструкторы класса.
    /// При применении контекста через метод <see cref="Apply"/> первый элемент коллекции
    /// передаётся как параметр <c>catalog_id</c>, а все элементы — как <c>catalog_ids</c>.
    /// </remarks>
    public ICollection<CatalogId> CatalogIds { get; }

    /// <summary>
    /// Инициализирует контекст для одного каталога с авторизацией по умолчанию для группы.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="catalogId">Идентификатор целевого каталога.</param>
    public GroupCatalogsRequestContext(GroupId groupId, CatalogId catalogId) : base(groupId)
    {
        CatalogIds = [catalogId];
    }

    /// <summary>
    /// Инициализирует контекст для одного каталога с явным указанием пары токенов доступа.
    /// </summary>
    /// <param name="accessPair">Пара токенов доступа (<c>access_token</c>, <c>session_secret_key</c>).</param>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="catalogId">Идентификатор целевого каталога.</param>
    public GroupCatalogsRequestContext(AccessPair accessPair, GroupId groupId, CatalogId catalogId) : base(accessPair, groupId)
    {
        CatalogIds = [catalogId];
    }
    
    /// <summary>
    /// Инициализирует контекст для нескольких каталогов с авторизацией по умолчанию.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="catalogIds">Массив идентификаторов каталогов. Не должен быть пустым.</param>
    public GroupCatalogsRequestContext(GroupId groupId, params CatalogId[] catalogIds) : base(groupId)
    {
        CatalogIds = catalogIds;
    }

    /// <summary>
    /// Инициализирует контекст для нескольких каталогов с явной парой токенов доступа.
    /// </summary>
    /// <param name="accessPair">Пара токенов доступа.</param>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="catalogIds">Массив идентификаторов каталогов.</param>
    public GroupCatalogsRequestContext(AccessPair accessPair, GroupId groupId, params CatalogId[] catalogIds) : base(accessPair, groupId)
    {
        CatalogIds = catalogIds; 
    }
    
    /// <summary>
    /// Применяет параметры контекста к объекту <see cref="RestParameters"/> перед отправкой запроса.
    /// </summary>
    /// <param name="parameters">Исходные параметры REST-запроса.</param>
    /// <returns>
    /// Модифицированный объект <see cref="RestParameters"/> с добавленными полями:
    /// <list type="bullet">
    /// <item><description><c>catalog_id</c> — значение первого элемента из <see cref="CatalogIds"/>.</description></item>
    /// <item><description><c>catalog_ids</c> — полный список идентификаторов из <see cref="CatalogIds"/>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// После добавления каталожных параметров вызывает базовую реализацию <see cref="GroupRequestContext.Apply"/>
    /// для применения стандартных групповых параметров (авторизация, идентификатор группы и т.д.).
    /// </remarks>
    public override RestParameters Apply(RestParameters parameters)
    {
        parameters.InsertCatalogId(CatalogIds.First().Value); 
        parameters.InsertCatalogIds(CatalogIds.Select(id => id.Value));

        return base.Apply(parameters);
    }
}