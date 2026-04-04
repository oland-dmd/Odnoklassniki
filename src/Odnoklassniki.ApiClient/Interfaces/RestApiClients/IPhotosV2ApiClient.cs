using Odnoklassniki.Rest.ApiClients.PhotosV2.Datas;
using Odnoklassniki.Rest.RequestContexts;

namespace Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с API фотографий версии 2 социальной сети Одноклассники (OK.ru).
/// Предоставляет методы для двухэтапной загрузки фотографий: получение временного URL для отправки файла
/// и подтверждение публикации после успешной загрузки. Работает через внешнее API OK.ru с использованием
/// контекста аутентификации <see cref="IRequestContext"/>.
/// </summary>
public interface IPhotosV2ApiClient
{
    /// <summary>
    /// Получает временный URL и служебные данные для загрузки фотографии в альбом текущего пользователя.
    /// </summary>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="albumId">
    /// Идентификатор целевого альбома в формате OK.ru. Если не указан или пустая строка,
    /// фотография загружается в основной альбом пользователя («Фотографии со страницы»).
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Объект <see cref="UploadUrlData"/>, содержащий:
    /// <list type="bullet">
    /// <item><description><c>UploadUrl</c> — HTTPS-адрес для отправки файла методом POST;</description></item>
    /// <item><description><c>PhotoId</c> — временный идентификатор загружаемого объекта;</description></item>
    /// <item><description><c>Token</c> — одноразовый токен для подтверждения публикации.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Возвращаемый URL действителен в течение ограниченного времени (обычно 15–30 минут).
    /// Фактическая отправка файла осуществляется клиентом напрямую на полученный адрес,
    /// минуя данный интерфейс. После загрузки необходимо вызвать <see cref="CommitAsync"/>
    /// для финализации публикации.
    /// <list type="bullet">
    /// <item><description>Требуются права на добавление фотографий в целевой альбом.</description></item>
    /// <item><description>Максимальный размер файла и поддерживаемые форматы определяются политикой OK.ru.</description></item>
    /// <item><description>При ошибке аутентификации или превышении квот выбрасывается соответствующее исключение.</description></item>
    /// </list>
    /// </remarks>
    Task<UploadUrlData> GetUploadUrlAsync(
        IRequestContext context,
        string? albumId = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Подтверждает загрузку фотографии и публикует её в альбоме с указанным комментарием.
    /// </summary>
    /// <param name="comment">Текст описания фотографии. Может быть пустым — в этом случае публикация происходит без подписи.</param>
    /// <param name="photoId">Временный идентификатор фотографии, полученный в ответе <see cref="GetUploadUrlAsync"/>.</param>
    /// <param name="token">Одноразовый токен подтверждения, полученный вместе с <c>photoId</c>.</param>
    /// <param name="context">Контекст запроса, содержащий данные аутентификации и авторизации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Коллекция объектов <see cref="CommitPhotoData"/> с результатами публикации.
    /// Для одиночной загрузки содержит один элемент с финальным идентификатором фотографии
    /// и ссылкой на опубликованный контент.
    /// </returns>
    /// <remarks>
    /// Метод должен вызываться только после успешной загрузки файла на URL, полученный через
    /// <see cref="GetUploadUrlAsync"/>. Параметры <c>photoId</c> и <c>token</c> автоматически
    /// кодируются в URL-формат для безопасной передачи в запросе к API.
    /// <list type="bullet">
    /// <item><description>Токен подтверждения одноразовый: повторный вызов с теми же данными вернёт ошибку.</description></item>
    /// <item><description>После успешного подтверждения фотография становится видимой в альбоме согласно настройкам приватности.</description></item>
    /// <item><description>При истечении времени жизни токена или некорректных данных метод выбрасывает исключение.</description></item>
    /// <item><description>Комментарий проходит модерацию платформы; контент, нарушающий правила, может быть скрыт или удалён.</description></item>
    /// </list>
    /// </remarks>
    Task<ICollection<CommitPhotoData>> CommitAsync(
        string comment,
        string photoId,
        string token,
        IRequestContext context,
        CancellationToken cancellationToken = default);
}