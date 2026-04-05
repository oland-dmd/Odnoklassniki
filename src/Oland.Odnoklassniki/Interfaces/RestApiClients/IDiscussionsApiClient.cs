using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент для взаимодействия с публичным API обсуждений (discussions) социальной сети «Одноклассники».
/// </summary>
public interface IDiscussionsApiClient
{
    /// <summary>
    /// Получает список обсуждений в группах, где участвует пользователь.
    /// </summary>
    /// <param name="context">Контекст, куда будет направлен запрос. Допустимые контексты: Основной</param>
    /// <param name="count">Количество записей (по умолчанию 100).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция обсуждений групп.</returns>
    Task<ICollection<DiscussionData>> GetGroupListAsync(
        ExplicitTokenRequestContext context,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список личных обсуждений пользователя.
    /// </summary>
    /// <param name="context">Контекст, куда будет направлен запрос. Допустимые контексты: С указанием токенов</param>
    /// <param name="count">Количество записей (по умолчанию 100).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// ///
    /// <returns>Коллекция личных обсуждений.</returns>
    Task<ICollection<DiscussionData>> GetUserListAsync(
        ExplicitTokenRequestContext context,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает комментарии к указанному обсуждению.
    /// </summary>
    /// <remarks>
    /// Поддерживает автоматическую отметку как прочитанных (mark_as_read=true).
    /// </remarks>
    /// <param name="context">Контекст, куда будет направлен запрос. Допустимые контексты: С указанием токенов</param>
    /// <param name="discussionId">Идентификатор обсуждения.</param>
    /// <param name="discussionType">Тип обсуждения (например, "ALBUM", "GROUP_POST").</param>
    /// <param name="count">Количество комментариев (по умолчанию 100).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция комментариев.</returns>
    Task<ICollection<CommentData>> GetCommentsAsync(
        ExplicitTokenRequestContext context,
        string discussionId,
        string discussionType,
        int count = 100,
        CancellationToken cancellationToken = default);
}