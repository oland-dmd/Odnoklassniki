using Oland.MediaManager.Application.Services;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Market.Models;
using Oland.Odnoklassniki.Rest.BeanFields;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент REST API Одноклассников для управления товарами в маркетплейсе.
/// </summary>
/// <remarks>
/// <para>
/// <b>Ответственность:</b> предоставление методов CRUD-операций и навигации по товарам
/// через API Одноклассников (методы класса <c>market</c>), включая сериализацию медиаконтента
/// через <see cref="IMediaService"/>.
/// </para>
/// <para>
/// <b>Источник данных:</b> внешнее API Одноклассников. Все запросы выполняются через
/// <c>IOkApiClientCore</c> с использованием контекста авторизации (<see cref="IRequestContext"/>).
/// </para>
/// <para>
/// <b>Зависимости:</b> 
/// <list type="bullet">
/// <item><description><see cref="IMediaService"/> — для сериализации данных товара в JSON-attachment.</description></item>
/// <item><description><c>IOkApiClientCore</c> — для отправки RPC-запросов к API Одноклассников.</description></item>
/// </list>
/// </para>
/// <para>
/// <b>Валидация:</b> модель <see cref="ProductModel"/> проверяется через свойство <c>IsValid</c>
/// перед отправкой. При ошибке валидации выбрасывается <see cref="ArgumentException"/>.
/// </para>
/// </remarks>
public interface IMarketProductsApiClient
{
    /// <summary>
    /// Создаёт новый товар через метод <c>market.add</c> с медиаконтентом в формате JSON-attachment.
    /// </summary>
    /// <param name="model">
    /// Модель товара с данными: название, описание, цена, валюта, изображения, партнёрская ссылка.
    /// Обязательные поля проверяются через <c>model.IsValid</c>.
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы:
    /// <list type="bullet">
    /// <item><description><see cref="GroupRequestContext"/>, <see cref="MainGroupRequestContext"/>, <see cref="GroupCatalogsRequestContext"/> — для товаров группы (<c>type=GROUP_PRODUCT</c>).</description></item>
    /// <item><description><see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/> — для личных товаров пользователя (<c>type=USER_PRODUCT</c>).</description></item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является строковый идентификатор созданного товара (<c>id</c>),
    /// или <c>null</c>, если ответ сервера не содержит данных.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.add</c>
    /// <br/><b>Формирование attachment:</b> данные сериализуются через <see cref="IMediaService.CreateAndSerialize"/>:
    /// текст названия и описания, фото по токенам, параметры товара (цена, валюта, срок жизни).
    /// <br/><b>Возвращаемые данные:</b> объект <c>AddEntityResponse</c> с полем <c>Id</c>.
    /// <br/><b>Побочные эффекты:</b> создание записи товара, индексация в поиске, модерация контента.
    /// </remarks>
    /// <response code="200">Товар успешно создан, возвращён его идентификатор.</response>
    /// <response code="400">Некорректные данные модели или параметры запроса.</response>
    /// <response code="403">Недостаточно прав для создания товара в указанном контексте.</response>
    /// <response code="500">Внутренняя ошибка сервера Одноклассников.</response>
    Task<string?> AddAsync(ProductModel model, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет данные существующего товара через метод <c>market.edit</c>.
    /// </summary>
    /// <param name="productId">Идентификатор редактируемого товара. Обязательное поле.</param>
    /// <param name="model">
    /// Обновлённая модель товара. Все поля, переданные в модели, перезаписывают текущие значения.
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы все типы контекстов маркетплейса:
    /// групповые (<see cref="GroupRequestContext"/>, <see cref="MainGroupRequestContext"/>, <see cref="GroupCatalogsRequestContext"/>)
    /// и пользовательские (<see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>).
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном обновлении
    /// (поле <c>success</c> в ответе), иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.edit</c>
    /// <br/><b>Формирование attachment:</b> аналогично методу <see cref="AddAsync"/>,
    /// полная замена медиаконтента товара.
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Побочные эффекты:</b> обновление метаданных, инвалидация кэша отображения,
    /// повторная модерация при изменении контента.
    /// </remarks>
    /// <response code="200">Товар успешно обновлён.</response>
    /// <response code="404">Товар с указанным <paramref name="productId"/> не найден.</response>
    /// <response code="403">Недостаточно прав для редактирования товара.</response>
    Task<bool> EditAsync(string productId, ProductModel model, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет товар через метод <c>market.delete</c>.
    /// </summary>
    /// <param name="productId">Идентификатор удаляемого товара. Обязательное поле.</param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы только пользовательские контексты:
    /// <see cref="MainAccountRequestContext"/> или <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном удалении, иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.delete</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Побочные эффекты:</b> удаление записи товара, удаление из всех каталогов,
    /// очистка индексов поиска, отзыв модерации.
    /// </remarks>
    /// <response code="200">Товар успешно удалён.</response>
    /// <response code="404">Товар не найден.</response>
    /// <response code="403">Недостаточно прав для удаления.</response>
    Task<bool> DeleteAsync(string productId, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для постраничного получения товаров из конкретного каталога
    /// через метод <c>market.getByCatalog</c> с поддержкой курсорной пагинации.
    /// </summary>
    /// <typeparam name="TDto">
    /// Тип возвращаемого DTO, наследующий <see cref="BaseOkDto"/>.
    /// Должен соответствовать структуре ответа API для кратких данных товара.
    /// </typeparam>
    /// <param name="context">
    /// Контекст запроса. Допустимы только групповые контексты:
    /// <see cref="GroupRequestContext"/>, <see cref="MainGroupRequestContext"/>, <see cref="GroupCatalogsRequestContext"/>.
    /// </param>
    /// <param name="anchorConfiguration">
    /// Конфигурация навигации: курсор (<c>anchor</c>), направление сортировки (<c>direction</c>),
    /// размер страницы (<c>count</c>). Используется для ленивой подгрузки данных.
    /// </param>
    /// <param name="fields">
    /// Опциональный список полей для проекции ответа. Если не указан, используется поле <c>id</c>
    /// по умолчанию (через <see cref="ShortProductBeanFields"/>).
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Экземпляр <see cref="AnchorNavigator{TDto}"/> для итерации по товарам каталога.
    /// Навигатор автоматически выполняет запросы при необходимости подгрузки следующей страницы.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.getByCatalog</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>AnchorResponse{TDto}</c> с полями
    /// <c>Anchor</c>, <c>HasMore</c>, <c>TotalCount</c>, <c>Results</c>.
    /// <br/><b>Особенности:</b> возвращаются только товары, непосредственно входящие в указанный каталог.
    /// </remarks>
    /// <response code="200">Страница данных успешно получена.</response>
    /// <response code="400">Некорректные параметры пагинации или идентификатор каталога.</response>
    /// <response code="403">Недостаточно прав для просмотра товаров каталога.</response>
    AnchorNavigator<TDto> GetByCatalogNavigator<TDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration, IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Возвращает навигатор для постраничного получения товаров с фильтрацией по вкладке
    /// через метод <c>market.getProducts</c> с поддержкой курсорной пагинации.
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, наследующий <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="context">
    /// Контекст запроса. Допустимы:
    /// <list type="bullet">
    /// <item><description>Групповые контексты — возвращают товары группы с фильтрацией по вкладке (<c>products</c> или <c>on_moderation</c>).</description></item>
    /// <item><description>Пользовательские контексты — возвращают только собственные товары пользователя (вкладка <c>own</c>).</description></item>
    /// </list>
    /// </param>
    /// <param name="anchorConfiguration">Конфигурация курсорной навигации.</param>
    /// <param name="fields">Опциональный список полей для проекции ответа.</param>
    /// <param name="onModeration">
    /// Флаг фильтрации товаров на модерации. Применяется только для групповых контекстов.
    /// Для пользовательских контекстов игнорируется (всегда возвращается вкладка <c>own</c>).
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Экземпляр <see cref="AnchorNavigator{TDto}"/> для итерации по результатам.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.getProducts</c>
    /// <br/><b>Параметр <c>tab</c>:</b> устанавливается автоматически в зависимости от контекста:
    /// <c>products</c>/<c>on_moderation</c> для групп, <c>own</c> для пользователей.
    /// <br/><b>Возвращаемые данные:</b> объект <c>AnchorResponse{TDto}</c>.
    /// </remarks>
    /// <response code="200">Страница данных успешно получена.</response>
    /// <response code="400">Некорректные параметры навигации.</response>
    /// <response code="403">Недостаточно прав для просмотра товаров.</response>
    AnchorNavigator<TDto> GetProductsNavigator<TDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration, IEnumerable<string>? fields = null,
        bool onModeration = false, CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    /// <summary>
    /// Получает полные данные товаров по списку идентификаторов через метод <c>market.getByIds</c>.
    /// </summary>
    /// <typeparam name="TDto">
    /// Тип возвращаемого DTO, наследующий <see cref="BaseOkDto"/>.
    /// Должен содержать маппинг для полей <c>media_text</c>, <c>id</c> и других запрошенных полей.
    /// </typeparam>
    /// <param name="productIds">Коллекция идентификаторов запрашиваемых товаров.</param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы только пользовательские контексты:
    /// <see cref="MainAccountRequestContext"/> или <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="fields">
    /// Опциональный список полей для включения в ответ. Если не указан, используются
    /// значения по умолчанию: <c>media_text</c>, <c>id</c> (через <see cref="MediaTopicBeanFields"/>).
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является коллекция DTO запрошенных товаров.
    /// Пустой список возвращается, если входная коллекция пуста или товары не найдены.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.getByIds</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>ProductsResponse{TDto}</c> с коллекцией <c>Products</c>.
    /// <br/><b>Особенности:</b> метод не выбрасывает исключение при частичном отсутствии товаров —
    /// в ответе возвращаются только найденные записи.
    /// </remarks>
    /// <response code="200">Данные успешно получены.</response>
    /// <response code="400">Некорректный формат идентификаторов или полей.</response>
    Task<ICollection<TDto>> GetByIdsAsync<TDto>(ICollection<string> productIds,
        IRequestContext context, IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Закрепляет или открепляет товар в верхней позиции списка через метод <c>market.pin</c>.
    /// </summary>
    /// <param name="productId">Идентификатор товара для закрепления.</param>
    /// <param name="on">
    /// Флаг действия: <c>true</c> — закрепить товар, <c>false</c> — открепить.
    /// </param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы:
    /// <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>,
    /// <see cref="GroupCatalogsRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном изменении статуса закрепления.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.pin</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Ограничения:</b> одновременно может быть закреплён только один товар в рамках каталога/профиля.
    /// </remarks>
    /// <response code="200">Статус закрепления успешно изменён.</response>
    /// <response code="404">Товар не найден.</response>
    /// <response code="403">Недостаточно прав для изменения статуса закрепления.</response>
    Task<bool> PinAsync(string productId, bool on, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменяет порядок отображения товаров внутри каталога или списка через метод <c>market.reorder</c>.
    /// </summary>
    /// <param name="productId">Идентификатор товара для перемещения.</param>
    /// <param name="context">
    /// Контекст выполнения запроса. Допустимы все типы контекстов маркетплейса.
    /// </param>
    /// <param name="afterProductId">
    /// Опциональный идентификатор товара, после которого следует разместить перемещаемый элемент
    /// (параметр <c>after_product_id</c>). Если <c>null</c>, товар перемещается в начало списка.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Задача, результатом которой является <c>true</c> при успешном изменении порядка.
    /// </returns>
    /// <remarks>
    /// <b>Метод API:</b> <c>market.reorder</c>
    /// <br/><b>Возвращаемые данные:</b> объект <c>CompletionStatusResponse</c>.
    /// <br/><b>Побочные эффекты:</b> обновление поля сортировки (<c>order_index</c>) у товаров,
    /// инвалидация кэша отображения списка.
    /// </remarks>
    /// <response code="200">Порядок товаров успешно обновлён.</response>
    /// <response code="400">Некорректная позиция перемещения (несуществующий <paramref name="afterProductId"/>).</response>
    /// <response code="404">Товар <paramref name="productId"/> не найден.</response>
    Task<bool> ReorderAsync(string productId, IRequestContext context,
        string? afterProductId = null, CancellationToken cancellationToken = default);
}