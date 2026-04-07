using Oland.Odnoklassniki.Rest.ApiClients.Market.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

public interface ICatalogsApiClient
{
    /// <summary>
    /// Добавление каталога товаров. Доступно только для группового контекста.
    /// </summary>
    /// <param name="title">Название каталог.</param>
    /// <param name="photoId">Идентификатор фото, полученный после загрузки фото. Загрузка фото происходит аналогично товарам и медиатопикам, без вызова метода commit.</param>
    /// <param name="adminRestricted">Флаг, добавлять товары может только администрация.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает id созданного каталога</returns>
    public Task<string?> AddCatalogAsync(string title, string photoId, bool adminRestricted,
        IRequestContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование каталога товаров. Доступно только для группового контекста.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога</param>
    /// <param name="title">Название каталог.</param>
    /// <param name="photoId">Идентификатор фото, полученный после загрузки фото. Загрузка фото происходит аналогично товарам и медиатопикам, без вызова метода commit.</param>
    /// <param name="adminRestricted">Флаг, добавлять товары может только администрация.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус созданного каталога</returns>
    public Task<bool> EditCatalogAsync(string catalogId, string title, string photoId, bool adminRestricted,
        IRequestContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление каталога из группы.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога.</param>
    /// <param name="deleteProducts">Флаг, удалить ли все товары из данного каталога.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус выполнения удаления.</returns>
    public Task<bool> DeleteCatalogAsync(string catalogId, bool deleteProducts, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Переместить каталоги внутри группы.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога для перемещения.</param>
    /// <param name="afterCatalogId">Ид каталога, после которого будет поставлен перемещаемый.</param>
    /// <param name="context">Контекст запроса в группу</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус выполнения</returns>
    public Task<bool> ReorderCatalogAsync(string catalogId, string afterCatalogId, IRequestContext context,
        CancellationToken cancellationToken = default);
    
    
    //TODO: Посмотреть как реализован динамический тип через запрашиваемые поля в альбомах и фото, придумать универсальный.
}