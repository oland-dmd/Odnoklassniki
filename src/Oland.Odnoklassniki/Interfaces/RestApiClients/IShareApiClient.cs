using Oland.Odnoklassniki.Rest.ApiClients.Share.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для работы с методами <c>share.*</c> API Одноклассники (OK.ru).
/// Позволяет получать информацию о внешних ссылках для последующей публикации в медиатопике.
/// </summary>
public interface IShareApiClient
{
    /// <summary>
    /// Получает информацию о ссылке для публикации в медиатопике (<c>share.fetchLinkV2</c>).
    /// </summary>
    /// <param name="url">URL страницы, о которой нужна информация. Обязательный параметр.</param>
    /// <param name="context">
    /// Контекст запроса. Принимает <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.
    /// </param>
    /// <param name="locale">Язык возвращаемых текстов (например, <c>ru</c>, <c>en</c>). Необязательно.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Объект <see cref="LinkInfoData"/> с заголовком, превью и <c>signature</c> для последующего
    /// использования в <c>mediatopic.post</c>, или <c>null</c>, если ответ сервера пуст.
    /// </returns>
    Task<LinkInfoData?> FetchLinkAsync(
        string url,
        IRequestContext context,
        string? locale = null,
        CancellationToken cancellationToken = default);
}
