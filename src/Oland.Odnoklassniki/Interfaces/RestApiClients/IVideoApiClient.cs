using Oland.Odnoklassniki.Rest.ApiClients.Video.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с видеороликами в социальной сети Одноклассники (OK.ru).
/// Поддерживает получение URL загрузки, обновление, удаление видео и подписку на канал.
/// </summary>
public interface IVideoApiClient
{
    /// <summary>
    /// Получает URL для загрузки нового видеоролика (<c>video.getUploadUrl</c>).
    /// </summary>
    /// <param name="fileName">Имя загружаемого файла (обязательно).</param>
    /// <param name="fileSize">Размер файла в байтах (обязательно; допустимо 0 при неизвестном размере).</param>
    /// <param name="context">Контекст запроса. Принимает <c>GroupRequestContext</c>, <c>MainGroupRequestContext</c>, <c>MainAccountRequestContext</c>, <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="attachmentType">Тип вложения: <c>MOVIE</c>, <c>VIDEO</c> или <c>AUDIO_RECORDING</c>. Необязательно.</param>
    /// <param name="channelId">Идентификатор канала (<c>cid</c>). Необязательно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Объект <see cref="VideoUploadUrlData"/> с URL загрузки и идентификатором видеоролика,
    /// или <c>null</c>, если ответ сервера пуст.
    /// </returns>
    Task<VideoUploadUrlData?> GetUploadUrlAsync(
        string fileName,
        long fileSize,
        IRequestContext context,
        string? attachmentType = null,
        string? channelId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет метаданные видеоролика (<c>video.update</c>).
    /// </summary>
    /// <param name="videoId">Идентификатор видеоролика (параметр <c>vid</c>).</param>
    /// <param name="context">Контекст запроса. Принимает <c>GroupRequestContext</c>, <c>MainGroupRequestContext</c>, <c>MainAccountRequestContext</c>, <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="title">Новое название. Необязательно.</param>
    /// <param name="description">Новое описание. Необязательно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task UpdateAsync(
        string videoId,
        IRequestContext context,
        string? title = null,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет видеоролик (<c>video.delete</c>).
    /// </summary>
    /// <param name="videoId">Идентификатор видеоролика (параметр <c>vid</c>).</param>
    /// <param name="context">Контекст запроса. Принимает <c>GroupRequestContext</c>, <c>MainGroupRequestContext</c>, <c>MainAccountRequestContext</c>, <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task DeleteAsync(
        string videoId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Подписывает текущего пользователя на видеоканал (<c>video.subscribe</c>).
    /// </summary>
    /// <param name="channelId">Идентификатор канала (<c>cid</c>).</param>
    /// <param name="context">Контекст запроса. Принимает <c>MainAccountRequestContext</c>, <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><c>true</c>, если подписка выполнена успешно.</returns>
    Task<bool> SubscribeAsync(
        string channelId,
        IRequestContext context,
        CancellationToken cancellationToken = default);
}
