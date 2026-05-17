using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с фотографиями в социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для загрузки, получения метаданных, редактирования и удаления фотографий.
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами через параметр <c>context</c>.
/// Все операции выполняются через внешнее API OK.ru с учётом квот и ограничений платформы.
/// </summary>
public interface IPhotosApiClient
{
    /// <summary>
    /// Возвращает навигатор для постраничного получения списка фотографий из указанного альбома.
    /// </summary>
    /// <param name="albumId">Идентификатор альбома в формате OK.ru.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="configuration">Настройки пагинации и якоря для навигации по коллекции.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Экземпляр <see cref="AnchorNavigator{PhotoData}"/> для итерации по фотографиям альбома
    /// с поддержкой курсорной пагинации.
    /// </returns>
    /// <remarks>
    /// Метод возвращает только фотографии, доступные для чтения текущему пользователю.
    /// По умолчанию запрашиваются базовые поля: идентификатор, URL превью, дата загрузки, владелец.
    /// <list type="bullet">
    /// <item><description>Пагинация реализуется через механизм anchor/cursor, а не через offset/limit.</description></item>
    /// <item><description>При отсутствии фотографий в альбоме навигатор возвращает пустую коллекцию.</description></item>
    /// <item><description>Для получения расширенных метаданных (геотеги, теги пользователей) используйте параметры конфигурации.</description></item>
    /// <item><description>При ошибке доступа к альбому или аутентификации навигатор выбросит исключение при первой итерации.</description></item>
    /// </list>
    /// </remarks>
    AnchorNavigator<PhotoData> GetPhotosNavigator(
        string albumId,
        IRequestContext context,
        AnchorConfiguration configuration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает подробную информацию о фотографии по её идентификатору.
    /// </summary>
    /// <param name="photoId">Идентификатор фотографии в формате OK.ru.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Объект <see cref="PhotoData"/> с полными метаданными фотографии,
    /// или выбрасывает исключение, если фотография не найдена или недоступна.
    /// </returns>
    /// <remarks>
    /// Метод требует наличия валидной сессии и прав на чтение контента.
    /// Возвращаемые данные включают: ссылки на изображения в разных разрешениях, дату загрузки,
    /// идентификатор альбома, информацию о владельце и настройки приватности.
    /// <list type="bullet">
    /// <item><description>Если фотография удалена или скрыта настройками приватности, метод выбрасывает исключение.</description></item>
    /// <item><description>Для массового получения данных о фотографиях предпочтительнее использовать пакетные методы, если доступны.</description></item>
    /// <item><description>Время ответа зависит от размера метаданных и доступности медиа-контента на стороне OK.ru.</description></item>
    /// </list>
    /// </remarks>
    Task<PhotoData> GetPhotoInfoAsync(string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Редактирует описание (подпись) указанной фотографии.
    /// </summary>
    /// <param name="photoId">Идентификатор редактируемой фотографии.</param>
    /// <param name="description">Новый текст описания. Пустая строка допустима для удаления подписи.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// <c>true</c>, если описание успешно обновлено; <c>false</c>, если изменения не были применены
    /// (например, из-за отсутствия прав или блокировки контента).
    /// </returns>
    /// <remarks>
    /// Операция требует прав на редактирование фотографии (владелец или модератор альбома).
    /// Максимальная длина описания определяется ограничениями платформы OK.ru.
    /// <list type="bullet">
    /// <item><description>Специальные символы и разметка в описании автоматически экранируются сервером.</description></item>
    /// <item><description>Изменения применяются асинхронно и могут стать видимыми с задержкой до нескольких секунд.</description></item>
    /// <item><description>При попытке редактирования чужой фотографии без прав метод возвращает <c>false</c>.</description></item>
    /// </list>
    /// </remarks>
    Task<bool> EditPhotoAsync(
        string photoId,
        string description,
        IRequestContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет фотографию текущего пользователя. Операция необратима.
    /// </summary>
    /// <param name="photoId">Идентификатор удаляемой фотографии.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <remarks>
    /// Внимание: удаление фотографии приводит к безвозвратной потере файла и всех связанных метаданных
    /// (комментарии, лайки, теги). Убедитесь в наличии резервной копии перед вызовом метода.
    /// <list type="bullet">
    /// <item><description>Операция требует прав владельца фотографии или модератора альбома.</description></item>
    /// <item><description>При успешном удалении все ссылки на фотографию возвращают ошибку 404.</description></item>
    /// <item><description>Метод не выбрасывает исключение при попытке удалить уже удалённую фотографию (идемпотентность).</description></item>
    /// </list>
    /// </remarks>
    Task DeletePhotoAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Пакетно получает информацию о фотографиях по списку идентификаторов (<c>photos.getInfo</c>).
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, унаследованный от <see cref="BaseOkDto"/>. Определяет набор запрашиваемых полей.</typeparam>
    /// <param name="photoIds">Список идентификаторов фотографий.</param>
    /// <param name="context">Контекст запроса с данными аутентификации.</param>
    /// <param name="fields">Запрашиваемые поля (из <c>PhotoBeanFields</c>). При <c>null</c> сервер возвращает набор по умолчанию.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция DTO с данными фотографий, или <c>null</c> при пустом ответе.</returns>
    Task<ICollection<TDto>?> GetPhotosInfoAsync<TDto>(
        ICollection<string> photoIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    /// <summary>
    /// Возвращает навигатор для перебора всех фотографий текущего пользователя (<c>photos.getUserPhotos</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод устарел на стороне OK API. Рекомендуется использовать <c>photos.getPhotos</c>.
    /// Использует пагинацию через <c>pagingAnchor</c>.
    /// </remarks>
    /// <typeparam name="TDto">Тип DTO, унаследованный от <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cfg">Настройки пагинации.</param>
    /// <param name="fields">Запрашиваемые поля.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор для курсорной итерации по фотографиям пользователя.</returns>
    AnchorNavigator<TDto> GetUserPhotosNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    /// <summary>
    /// Возвращает навигатор для перебора фотографий из конкретного альбома пользователя (<c>photos.getUserAlbumPhotos</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод устарел на стороне OK API. Рекомендуется использовать <c>photos.getPhotos</c>.
    /// Параметр альбома передаётся как <c>aid</c>. Использует пагинацию через <c>pagingAnchor</c>.
    /// </remarks>
    /// <typeparam name="TDto">Тип DTO, унаследованный от <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="albumId">Идентификатор альбома.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cfg">Настройки пагинации.</param>
    /// <param name="fields">Запрашиваемые поля.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор для курсорной итерации по фотографиям альбома.</returns>
    AnchorNavigator<TDto> GetUserAlbumPhotosNavigator<TDto>(
        string albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default)
        where TDto : BaseOkDto;

    /// <summary>
    /// Ставит лайк («Класс») на фотографию (<c>photos.addPhotoLike</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Поддержка метода завершена на стороне OK API. Требует scope <c>LIKE</c> + <c>PHOTO_CONTENT</c> + <c>VALUABLE_ACCESS</c>.
    /// Нельзя ставить лайк своим фотографиям.
    /// </remarks>
    /// <param name="photoId">Идентификатор фотографии.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c> при успешной постановке лайка.</returns>
    Task<bool> AddPhotoLikeAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ставит лайк («Класс») на альбом (<c>photos.addAlbumLike</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Поддержка метода завершена на стороне OK API. Требует scope <c>LIKE</c> + <c>PHOTO_CONTENT</c> + <c>VALUABLE_ACCESS</c>.
    /// Параметр альбома: <c>aid</c>. Нельзя ставить лайк своим альбомам.
    /// </remarks>
    /// <param name="albumId">Идентификатор альбома.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c> при успешной постановке лайка.</returns>
    Task<bool> AddAlbumLikeAsync(
        string albumId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для перебора пользователей, поставивших лайк на фотографию (<c>photos.getPhotoLikes</c>).
    /// </summary>
    /// <param name="photoId">Идентификатор фотографии.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cfg">Настройки пагинации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор по пользователям.</returns>
    AnchorNavigator<UserLikeDto> GetPhotoLikesNavigator(
        string photoId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для перебора пользователей, поставивших лайк на альбом (<c>photos.getAlbumLikes</c>).
    /// </summary>
    /// <param name="albumId">Идентификатор альбома.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cfg">Настройки пагинации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор по пользователям.</returns>
    AnchorNavigator<UserLikeDto> GetAlbumLikesNavigator(
        string albumId,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает оценки (marks), выставленные текущим пользователем чужим фотографиям (<c>photos.getPhotoMarks</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод устарел на стороне OK API. Возвращает все оценки текущего пользователя — параметр <c>photo_id</c> отсутствует.
    /// </remarks>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция оценок или <c>null</c> при пустом ответе.</returns>
    Task<ICollection<PhotoMarkDto>?> GetPhotoMarksAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает теги пользователей на фотографии (<c>photos.getTags</c>).
    /// </summary>
    /// <param name="photoId">Идентификатор фотографии.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция тегов или <c>null</c> при пустом ответе.</returns>
    Task<ICollection<PhotoTagDto>?> GetTagsAsync(
        string photoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет теги пользователей на фотографии (<c>photos.deleteTags</c>).
    /// </summary>
    /// <remarks>
    /// Максимум 10 тегов за один вызов. Фотография должна принадлежать текущему пользователю.
    /// </remarks>
    /// <param name="photoId">Идентификатор фотографии текущего пользователя.</param>
    /// <param name="tagIds">Список идентификаторов тегов для удаления (макс. 10).</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Количество фактически удалённых тегов.</returns>
    Task<int> DeleteTagsAsync(
        string photoId,
        ICollection<string> tagIds,
        IRequestContext context,
        CancellationToken cancellationToken = default);
}