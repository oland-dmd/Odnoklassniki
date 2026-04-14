using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

public interface IMarketCatalogsApiClient
{
    /// <summary>
    /// Добавление каталога товаров. Доступно только для группового контекста.
    /// </summary>
    /// <param name="title">Название каталог.</param>
    /// <param name="adminRestricted">Флаг, добавлять товары может только администрация.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="photoId">Идентификатор фото, полученный после загрузки фото. Загрузка фото происходит аналогично товарам и медиатопикам, без вызова метода commit.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает id созданного каталога</returns>
    public Task<string?> AddAsync(string title, bool adminRestricted, IRequestContext context,
        string? photoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование каталога товаров. Доступно только для группового контекста.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога</param>
    /// <param name="title">Название каталог.</param>
    /// <param name="adminRestricted">Флаг, добавлять товары может только администрация.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="photoId">Идентификатор фото, полученный после загрузки фото. Загрузка фото происходит аналогично товарам и медиатопикам, без вызова метода commit.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус созданного каталога</returns>
    public Task<bool> EditAsync(string catalogId, string title, bool adminRestricted,
        IRequestContext context, string? photoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаление каталога из группы.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога.</param>
    /// <param name="deleteProducts">Флаг, удалить ли все товары из данного каталога.</param>
    /// <param name="context">Контекст выполнения запроса. Доступно только для GroupRequestContext и MainGroupRequestContext.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус выполнения удаления.</returns>
    public Task<bool> DeleteAsync(string catalogId, bool deleteProducts, IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Переместить каталоги внутри группы.
    /// </summary>
    /// <param name="catalogId">Идентификатор каталога для перемещения.</param>
    /// <param name="context">Контекст запроса в группу</param>
    /// <param name="afterCatalogId">Ид каталога, после которого будет поставлен перемещаемый.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Возвращает статус выполнения</returns>
    public Task<bool> ReorderAsync(string catalogId, IRequestContext context, string? afterCatalogId = null,
        CancellationToken cancellationToken = default);

    Task<ICollection<TDto>> GetByIdsAsync<TDto>(IEnumerable<string> catalogIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    AnchorNavigator<TDto> GetByGroupAnchorNavigator<TDto>(IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;
}