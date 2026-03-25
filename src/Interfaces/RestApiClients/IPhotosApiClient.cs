using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Photos.Datas;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с фотографиями в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public interface IPhotosApiClient
{
    #region Get Photos List

    // === Для основного аккаунта ===

    /// <summary>
    /// Получает список фотографий из альбома текущего пользователя (основной аккаунт).
    /// </summary>
    AnchorNavigator<PhotoData> GetUserPhotosAsync(
        string albumId,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список фотографий из альбома друга (основной аккаунт).
    /// </summary>
    AnchorNavigator<PhotoData> GetFriendPhotosAsync(
        string albumId,
        string friendId,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список фотографий из альбома группы (основной аккаунт).
    /// </summary>
    AnchorNavigator<PhotoData> GetGroupPhotosAsync(
        string albumId,
        string groupId,
        int count = 100,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Получает список фотографий из альбома пользователя с указанными токенами.
    /// </summary>
    AnchorNavigator<PhotoData> GetUserPhotosAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список фотографий из альбома друга с указанными токенами.
    /// </summary>
    AnchorNavigator<PhotoData> GetFriendPhotosAsync(string accessToken,
        string sessionSecretKey,
        string albumId,
        string friendId,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список фотографий из альбома группы с указанными токенами.
    /// </summary>
    AnchorNavigator<PhotoData> GetGroupPhotosAsync(string accessToken,
        string sessionSecretKey,
        string albumId,
        string groupId,
        int count = 100,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get Photo Info

    // === Для основного аккаунта ===

    /// <summary>
    /// Получает информацию о фотографии пользователя (основной аккаунт).
    /// </summary>
    Task<PhotoData> GetUserPhotoInfoAsync(
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает информацию о фотографии группы (основной аккаунт).
    /// </summary>
    Task<PhotoData> GetGroupPhotoInfoAsync(
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Получает информацию о фотографии пользователя с указанными токенами.
    /// </summary>
    Task<PhotoData> GetUserPhotoInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает информацию о фотографии группы с указанными токенами.
    /// </summary>
    Task<PhotoData> GetGroupPhotoInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Edit Photo

    // === Для основного аккаунта ===

    /// <summary>
    /// Редактирует описание фотографии пользователя (основной аккаунт).
    /// </summary>
    Task<bool> EditUserPhotoAsync(
        string photoId,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирует описание фотографии группы (основной аккаунт).
    /// </summary>
    Task<bool> EditGroupPhotoAsync(
        string photoId,
        string description,
        string groupId,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Редактирует описание фотографии пользователя с указанными токенами.
    /// </summary>
    Task<bool> EditUserPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирует описание фотографии группы с указанными токенами.
    /// </summary>
    Task<bool> EditGroupPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string description,
        string groupId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Delete Photo

    // === Для основного аккаунта ===

    /// <summary>
    /// Удаляет фотографию пользователя (основной аккаунт, необратимо).
    /// </summary>
    Task DeleteUserPhotoAsync(
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет фотографию группы (основной аккаунт, необратимо).
    /// </summary>
    Task DeleteGroupPhotoAsync(
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Удаляет фотографию пользователя с указанными токенами (необратимо).
    /// </summary>
    Task DeleteUserPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет фотографию группы с указанными токенами (необратимо).
    /// </summary>
    Task DeleteGroupPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string photoId,
        string groupId,
        CancellationToken cancellationToken = default);

    #endregion
}