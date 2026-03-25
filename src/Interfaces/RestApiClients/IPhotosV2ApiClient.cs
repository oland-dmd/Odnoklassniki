using Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с API фотографий (версия 2) социальной сети Одноклассники (OK.ru).
/// </summary>
public interface IPhotosV2ApiClient
{
    #region Upload URL Methods

    /// <summary>
    /// Получает URL для загрузки фотографии в основной альбом текущего пользователя.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные для загрузки: URL и временные идентификаторы фото.</returns>
    Task<UploadUrlData> GetUploadUrlForUserAsync(
        string accessToken,
        string sessionSecretKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает URL для загрузки фотографии в указанный альбом текущего пользователя.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="albumId">Идентификатор альбома пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные для загрузки: URL и временные идентификаторы фото.</returns>
    Task<UploadUrlData> GetUploadUrlForUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает URL для загрузки фотографии в основной альбом группы.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="groupId">Идентификатор группы (сообщества).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные для загрузки: URL и временные идентификаторы фото.</returns>
    Task<UploadUrlData> GetUploadUrlForGroupAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает URL для загрузки фотографии в указанный альбом группы.
    /// </summary>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="groupId">Идентификатор группы (сообщества).</param>
    /// <param name="albumId">Идентификатор альбома внутри группы.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные для загрузки: URL и временные идентификаторы фото.</returns>
    Task<UploadUrlData> GetUploadUrlForGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default);

    #endregion

    /// <summary>
    /// Подтверждает загрузку фотографии и публикует её с комментарием.
    /// </summary>
    /// <remarks>
    /// Параметры photoId и token должны быть получены после фактической загрузки файла на полученный ранее URL.
    /// Внутри метода параметры кодируются в URL-формат для корректной передачи.
    /// </remarks>
    /// <param name="accessToken">Токен доступа пользователя.</param>
    /// <param name="sessionSecretKey">Секретный ключ сессии для подписи запроса.</param>
    /// <param name="comment">Комментарий к фотографии (может быть пустым).</param>
    /// <param name="photoId">Идентификатор загруженной фотографии, полученный от сервера.</param>
    /// <param name="token">Временный токен загрузки, полученный вместе с upload URL.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция с результатом подтверждения для каждого фото.</returns>
    Task<ICollection<CommitPhotoData>> CommitAsync(
        string accessToken,
        string sessionSecretKey,
        string comment,
        string photoId,
        string token,
        CancellationToken cancellationToken = default);
}