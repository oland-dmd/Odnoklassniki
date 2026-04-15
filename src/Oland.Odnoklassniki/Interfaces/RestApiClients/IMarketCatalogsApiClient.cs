using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Exceptions;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент REST API Одноклассников для управления каталогами товаров в маркетплейсе.
/// </summary>
/// <remarks>
/// <para>
/// <b>Ответственность:</b> предоставление методов для создания, редактирования, удаления и получения
/// каталогов товаров через API Одноклассников (методы класса <c>market</c>).
/// </para>
/// <para>
/// <b>Источник данных:</b> внешнее API Одноклассников. Все запросы выполняются через <c>IOkApiClientCore</c>
/// с использованием контекста авторизации (<see cref="IRequestContext"/>).
/// </para>
/// <para>
/// <b>Требования к контексту:</b> все методы работают исключительно в групповом контексте:
/// <see cref="GroupRequestContext"/> или <see cref="MainGroupRequestContext"/>.
/// При передаче иного контекста выбрасывается <see cref="UnexpectedRequestContext"/>.
/// </para>
/// <para>
/// <b>Валидация:</b> проверка входных параметров выполняется на стороне сервера Одноклассников.
/// Рекомендуется предварительная валидация названия каталога (не пустое, ограничение длины)
/// и идентификаторов (формат строки, допустимые символы).
/// </para>
/// </remarks>
public interface IMarketCatalogsApiClient
{
    /// <summary>
    /// Создаёт новый каталог товаров в группе через метод <c>market.addCatalog</c>.
    /// </summary>
    /// <param name="title">Название каталога. Обязательное поле, передаётся как параметр <c>name</c>.</param>
    /// <param name="adminRestricted">
    /// Флаг ограничения прав на добавление товаров: <c>true</c> — только администраторы группы
    /// (параметр <c>admin_restricted</c>), <c>false</c> — разрешено участникам с правами.
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы только <see cref="GroupRequestContext"/>
    /// и <see cref="MainGroupRequestContext"/>. Содержит данные авторизации и идентификатор группы.
    /// </param>
    /// <param name="photoId">
    /// Опциональный идентификатор обложки каталога. Фото должно быть предварительно загружено
    /// через медиасервис Одноклассников без вызова <c>commit</c>. Передаётся как параметр <c>photo_id</c>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является строковый идентификатор созданного каталога (<c>id</c>),
    /// или <c>null</c>, если ответ сервера не содержит данных.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.addCatalog</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>AddEntityResponse</c> с полем <c>Id</c>.
    /// <br/><b>Побочные эффекты:</b> создание записи в базе данных Одноклассников, индексация каталога.
    /// </remarks>
    /// <response code="200">Каталог успешно создан, возвращён его идентификатор.</response>
    /// <response code="400">Некорректные параметры запроса (пустое название, неверный формат photoId).</response>
    /// <response code="403">Недостаточно прав для создания каталога в указанной группе.</response>
    /// <response code="500">Внутренняя ошибка сервера Одноклассников.</response>
    Task<string?> AddAsync(string title, bool adminRestricted, IRequestContext context,
        string? photoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет параметры существующего каталога через метод <c>market.editCatalog</c>.
    /// </summary>
    /// <param name="catalogId">Идентификатор редактируемого каталога. Обязательное поле (<c>catalog_id</c>).</param>
    /// <param name="title">Новое название каталога (параметр <c>name</c>).</param>
    /// <param name="adminRestricted">
    /// Обновлённое значение флага ограничения прав на добавление товаров (<c>admin_restricted</c>).
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы только <see cref="GroupRequestContext"/>
    /// и <see cref="MainGroupRequestContext"/>.
    /// </param>
    /// <param name="photoId">
    /// Опциональный идентификатор новой обложки. Если передан, заменяет текущее изображение каталога.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном обновлении
    ///(поле <c>success</c> в ответе), иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.editCatalog</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c> с флагом <c>Success</c>.
    /// <br/><b>Побочные эффекты:</b> обновление метаданных каталога, инвалидация кэша отображения.
    /// </remarks>
    /// <response code="200">Каталог успешно обновлён.</response>
    /// <response code="404">Каталог с указанным <paramref name="catalogId"/> не найден.</response>
    /// <response code="403">Недостаточно прав для редактирования каталога.</response>
    Task<bool> EditAsync(string catalogId, string title, bool adminRestricted,
        IRequestContext context, string? photoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет каталог товаров из группы через метод <c>market.deleteCatalog</c>.
    /// </summary>
    /// <param name="catalogId">Идентификатор удаляемого каталога. Обязательное поле.</param>
    /// <param name="deleteProducts">
    /// Флаг каскадного удаления: <c>true</c> — удалить все товары, входящие в каталог
    /// (параметр <c>delete_products</c>); <c>false</c> — товары сохраняются в общем пуле группы.
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы только <see cref="GroupRequestContext"/>
    /// и <see cref="MainGroupRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном удалении, иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.deleteCatalog</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Побочные эффекты:</b> удаление записи каталога, опциональное удаление связанных товаров,
    /// обновление индексов поиска.
    /// </remarks>
    /// <response code="200">Каталог успешно удалён.</response>
    /// <response code="404">Каталог не найден.</response>
    /// <response code="403">Недостаточно прав для удаления.</response>
    Task<bool> DeleteAsync(string catalogId, bool deleteProducts, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменяет порядок отображения каталогов внутри группы через метод <c>market.reorderCatalogs</c>.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога для перемещения.</param>
    /// <param name="context">Контекст запроса в группу.</param>
    /// <param name="afterCatalogId">
    /// Опциональный идентификатор каталога, после которого следует разместить перемещаемый элемент
    /// (параметр <c>after_catalog_id</c>). Если <c>null</c>, каталог перемещается в начало списка.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном изменении порядка, иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.reorderCatalogs</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Побочные эффекты:</b> обновление поля сортировки (<c>order_index</c>) у каталогов,
    /// инвалидация кэша отображения списка.
    /// </remarks>
    /// <response code="200">Порядок каталогов успешно обновлён.</response>
    /// <response code="400">Некорректная позиция перемещения (несуществующий <paramref name="afterCatalogId"/>).</response>
    /// <response code="404">Каталог <paramref name="catalogId"/> не найден.</response>
    Task<bool> ReorderAsync(string catalogId, IRequestContext context, string? afterCatalogId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает данные каталогов по списку идентификаторов через метод <c>market.getCatalogsByIds</c>.
    /// </summary>
    /// <typeparam name="TDto">
    /// Тип возвращаемого DTO, наследующий <see cref="BaseOkDto"/>.
    /// Должен соответствовать структуре ответа API и содержать необходимые маппинги свойств.
    /// </typeparam>
    /// <param name="catalogIds">Коллекция идентификаторов запрашиваемых каталогов.</param>
    /// <param name="context">Контекст выполнения запроса.</param>
    /// <param name="fields">
    /// Опциональный список полей для включения в ответ (параметр <c>fields</c>).
    /// Если не указан, используются значения по умолчанию:
    /// <c>user_id</c>, <c>name</c>, <c>capabilities</c> (через <see cref="CatalogBeanFields"/>).
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является коллекция DTO запрошенных каталогов.
    /// Пустой список возвращается, если входная коллекция пуста или каталоги не найдены.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.getCatalogsByIds</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CatalogsResponse{TDto}</c> с коллекцией <c>Catalogs</c>.
    /// <br/><b>Особенности:</b> метод не выбрасывает исключение при частичном отсутствии каталогов —
    /// в ответе возвращаются только найденные записи.
    /// </remarks>
    /// <response code="200">Данные успешно получены.</response>
    /// <response code="400">Некорректный формат идентификаторов или полей.</response>
    Task<ICollection<TDto>> GetByIdsAsync<TDto>(ICollection<string> catalogIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    /// <summary>
    /// Возвращает навигатор для постраничного получения каталогов группы
    /// через метод <c>market.getCatalogsByGroup</c> с поддержкой курсорной пагинации.
    /// </summary>
    /// <typeparam name="TCatalogDto">Тип DTO, наследующий <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="context">Контекст запроса в группу.</param>
    /// <param name="anchorConfiguration">
    /// Конфигурация навигации: курсор (<c>anchor</c>), направление сортировки (<c>direction</c>),
    /// размер страницы (<c>count</c>). Используется для ленивой подгрузки данных.
    /// </param>
    /// <param name="fields">
    /// Опциональный список полей для проекции ответа. По умолчанию:
    /// <c>user_id</c>, <c>name</c>, <c>capabilities</c>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Экземпляр <see cref="AnchorNavigator{TCatalogDto}"/> для итерации по результатам.
    /// Навигатор автоматически выполняет запросы при необходимости подгрузки следующей страницы.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.getCatalogsByGroup</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>AnchorResponse{TCatalogDto}</c> с полями
    /// <c>Anchor</c>, <c>HasMore</c>, <c>TotalCount</c>, <c>Results</c>.
    /// <br/><b>Особенности:</b> навигатор кэширует уже загруженные страницы в рамках жизненного цикла экземпляра.
    /// </remarks>
    /// <response code="200">Страница данных успешно получена.</response>
    /// <response code="400">Некорректные параметры пагинации или полей.</response>
    /// <response code="403">Недостаточно прав для просмотра каталогов группы.</response>
    AnchorNavigator<TCatalogDto> GetByGroupAnchorNavigator<TCatalogDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TCatalogDto : BaseOkDto;
}