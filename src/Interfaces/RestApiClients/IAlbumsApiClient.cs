using Odnoklassniki.Enums;
using Odnoklassniki.Rest.AnchorNavigators.Anchor;
using Odnoklassniki.Rest.ApiClients.Photos.Datas;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для управления альбомами фотографий в социальной сети Одноклассники (OK.ru).
/// Поддерживает работу как с основным аккаунтом (настроенным в IOkApiClientCore),
/// так и с произвольными пользовательскими токенами.
/// </summary>
public interface IAlbumsApiClient
{
    #region Set Main Photo

    // === Для основного аккаунта ===

    /// <summary>
    /// Устанавливает фотографию как обложку альбома текущего пользователя (основной аккаунт).
    /// </summary>
    Task SetUserAlbumMainPhotoAsync(
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Устанавливает фотографию как обложку альбома группы (основной аккаунт).
    /// </summary>
    Task SetGroupAlbumMainPhotoAsync(
        string groupId,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Устанавливает фотографию как обложку альбома пользователя с указанными токенами.
    /// </summary>
    Task SetUserAlbumMainPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Устанавливает фотографию как обложку альбома группы с указанными токенами.
    /// </summary>
    Task SetGroupAlbumMainPhotoAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        string photoId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Create Album

    // === Для основного аккаунта ===

    /// <summary>
    /// Создаёт новый публичный альбом в профиле текущего пользователя (основной аккаунт).
    /// </summary>
    /// <returns>Идентификатор созданного альбома.</returns>
    Task<string> CreateUserAlbumAsync(string title,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый публичный альбом в группе (основной аккаунт).
    /// </summary>
    /// <returns>Идентификатор созданного альбома.</returns>
    Task<string> CreateGroupAlbumAsync(string groupId,
        string title,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Создаёт новый публичный альбом в профиле пользователя с указанными токенами.
    /// </summary>
    /// <returns>Идентификатор созданного альбома.</returns>
    Task<string> CreateUserAlbumAsync(string accessToken,
        string sessionSecretKey,
        string title,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый публичный альбом в группе с указанными токенами.
    /// </summary>
    /// <returns>Идентификатор созданного альбома.</returns>
    Task<string> CreateGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string title,
        CancellationToken cancellationToken = default);

    #endregion

    #region Delete Album

    // === Для основного аккаунта ===

    /// <summary>
    /// Удаляет альбом текущего пользователя (основной аккаунт, необратимо).
    /// </summary>
    Task DeleteUserAlbumAsync(
        string albumId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет альбом группы (основной аккаунт, необратимо).
    /// </summary>
    Task DeleteGroupAlbumAsync(
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Удаляет альбом пользователя с указанными токенами (необратимо).
    /// </summary>
    Task DeleteUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет альбом группы с указанными токенами (необратимо).
    /// </summary>
    Task DeleteGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Edit Album

    // === Для основного аккаунта ===

    /// <summary>
    /// Редактирует название и описание альбома текущего пользователя (основной аккаунт).
    /// </summary>
    Task EditUserAlbumAsync(
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирует название и описание альбома группы (основной аккаунт).
    /// </summary>
    Task EditGroupAlbumAsync(
        string groupId,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default);

    // === С явными токенами ===

    /// <summary>
    /// Редактирует название и описание альбома пользователя с указанными токенами.
    /// </summary>
    Task EditUserAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирует название и описание альбома группы с указанными токенами.
    /// </summary>
    Task EditGroupAlbumAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        string title,
        string description,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get Albums List

    // === Для основного аккаунта ===

    /// <summary>
    /// Получает список альбомов текущего пользователя (основной аккаунт) с поддержкой пагинации.
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: album.title, album.aid, album.user_id, album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    AnchorNavigator<AlbumData> GetUserAlbumsAsync(string anchor = "", int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает список альбомов группы (основной аккаунт) с поддержкой пагинации.
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: group_album.title, group_album.aid, group_album.user_id, group_album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    AnchorNavigator<AlbumData> GetGroupAlbumsNavigator(string groupId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields);

    // === С явными токенами ===

    /// <summary>
    /// Получает список альбомов пользователя с указанными токенами и поддержкой пагинации.
    /// </summary>
    AnchorNavigator<AlbumData> GetUserAlbumsAsync(string accessToken,
        string sessionSecretKey,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает список альбомов друга с указанными токенами и поддержкой пагинации.
    /// </summary>
    AnchorNavigator<AlbumData> GetFriendAlbumsAsync(string accessToken,
        string sessionSecretKey,
        string friendId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает список альбомов группы с указанными токенами и поддержкой пагинации.
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: group_album.title, group_album.aid, group_album.user_id, group_album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    AnchorNavigator<AlbumData> GetGroupAlbumsNavigator(string accessToken,
        string sessionSecretKey,
        string groupId,
        string anchor = "",
        int count = 100,
        PagingDirection direction = PagingDirection.AROUND,
        CancellationToken cancellationToken = default,
        params string[] fields);

    #endregion

    #region Get Album Info

    // === Для основного аккаунта ===

    /// <summary>
    /// Получает подробную информацию об альбоме текущего пользователя (основной аккаунт).
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: album.title, album.aid, album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    Task<AlbumData?> GetUserAlbumInfoAsync(
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает подробную информацию об альбоме группы (основной аккаунт).
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: group_album.title, group_album.aid, group_album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    Task<AlbumData?> GetGroupAlbumInfoAsync(
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields);

    // === С явными токенами ===

    /// <summary>
    /// Получает подробную информацию об альбоме текущего пользователя с указанными токенами.
    /// </summary>
    Task<AlbumData?> GetUserAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает подробную информацию об альбоме друга с указанными токенами.
    /// </summary>
    Task<AlbumData?> GetFriendAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string albumId,
        string friendId,
        CancellationToken cancellationToken = default,
        params string[] fields);

    /// <summary>
    /// Получает подробную информацию об альбоме группы с указанными токенами.
    /// </summary>
    /// <remarks>
    /// По умолчанию запрашиваются поля: group_album.title, group_album.aid, group_album.ADD_PHOTO_ALLOWED.
    /// </remarks>
    Task<AlbumData?> GetGroupAlbumInfoAsync(
        string accessToken,
        string sessionSecretKey,
        string groupId,
        string albumId,
        CancellationToken cancellationToken = default,
        params string[] fields);

    #endregion
}