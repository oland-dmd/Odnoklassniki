using Odnoklassniki.Rest.AnchorNavigators;
using Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Odnoklassniki.Rest.RequestContexts;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для управления альбомами фотографий в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами через параметр <c>context</c>.
/// </summary>
public interface IAlbumsApiClient
{
    /// <summary>
    /// Устанавливает указанную фотографию в качестве обложки альбома указанного контекста.
    /// </summary>
    /// <param name="albumId">Идентификатор альбома в формате OK.ru.</param>
    /// <param name="photoId">Идентификатор фотографии, которая станет обложкой.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт и указанный токен</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <remarks>
    /// Операция требует прав на редактирование альбома. При ошибке аутентификации или отсутствии прав
    /// метод выбрасывает соответствующее исключение. Изменения применяются немедленно.
    /// </remarks>
    Task SetAlbumMainPhotoAsync(string albumId,
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый публичный альбом в профиле указанного контекста.
    /// </summary>
    /// <param name="title">Название альбома (обязательное поле).</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт и указанный токен</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданного альбома в формате OK.ru.</returns>
    /// <remarks>
    /// Альбом создаётся с настройками видимости «Публичный». Для изменения параметров доступа
    /// используйте метод <see cref="EditAlbumAsync"/>. Максимальная длина названия определяется
    /// ограничениями платформы OK.ru.
    /// </remarks>
    Task<string> CreateAlbumAsync(string title,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет альбом указанного контекста. Операция необратима.
    /// </summary>
    /// <param name="albumId">Идентификатор удаляемого альбома.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт и указанный токен</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <remarks>
    /// Внимание: удаление альбома приводит к безвозвратной потере всех содержащихся в нём фотографий
    /// и метаданных. Убедитесь в наличии резервной копии данных перед вызовом метода.
    /// </remarks>
    Task DeleteAlbumAsync(string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Редактирует метаданные альбома: название и описание.
    /// </summary>
    /// <param name="albumId">Идентификатор редактируемого альбома.</param>
    /// <param name="title">Новое название альбома.</param>
    /// <param name="description">Новое описание альбома.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт и указанный токен</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <remarks>
    /// Передача пустой строки допустима для поля <c>description</c>. Поле <c>title</c> является обязательным
    /// и должно соответствовать ограничениям платформы по длине и допустимым символам.
    /// </remarks>
    Task EditAlbumAsync(string albumId,
        string title,
        string description,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Возвращает навигатор для постраничного получения списка альбомов указанного контекста.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт, указанный токен и друга</param>
    /// <param name="configuration">Настройки пагинации и якоря для навигации по коллекции.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <param name="fields">Список полей альбома для включения в ответ (опционально).</param>
    /// <returns>Экземпляр <see cref="AnchorNavigator{AlbumData}"/> для итерации по результатам.</returns>
    /// <remarks>
    /// По умолчанию запрашиваются поля: <c>album.title</c>, <c>album.aid</c>, <c>album.user_id</c>, 
    /// <c>album.ADD_PHOTO_ALLOWED</c>. Для получения дополнительных данных передайте их названия 
    /// в параметре <c>fields</c> согласно документации OK.ru API.
    /// </remarks>
    AnchorNavigator<AlbumData> GetAlbumsNavigatorAsync(IRequestContext context,
        AnchorConfiguration configuration,
        CancellationToken cancellationToken = default,
        params string[] fields);
    
    /// <summary>
    /// Получает подробную информацию об указанном альбоме указанного контекста.
    /// </summary>
    /// <param name="albumId">Идентификатор запрашиваемого альбома.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации. Поддерживает контексты: групповой, основной аккаунт, указанный токен и друга</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <param name="fields">Список полей альбома для включения в ответ (опционально).</param>
    /// <returns>Объект <see cref="AlbumData"/> с данными альбома, или <c>null</c>, если альбом не найден.</returns>
    /// <remarks>
    /// По умолчанию запрашиваются поля: <c>album.title</c>, <c>album.aid</c>, <c>album.ADD_PHOTO_ALLOWED</c>.
    /// Если альбом не принадлежит текущему пользователю или недоступен по настройкам приватности,
    /// метод возвращает <c>null</c> или выбрасывает исключение в зависимости от политики обработки ошибок.
    /// </remarks>
    Task<AlbumData?> GetAlbumInfoAsync(string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default,
        params string[] fields);
}