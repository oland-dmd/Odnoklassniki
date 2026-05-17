using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Enums;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;
using Oland.Odnoklassniki.Rest.RequestContexts;
using Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

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

    // ─── Новые методы (фаза 7) ───────────────────────────────────────────────

    /// <summary>
    /// Возвращает навигатор для постраничного перебора списка обсуждений (<c>discussions.getList</c>).
    /// </summary>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cfg">Конфигурация курсорной пагинации.</param>
    /// <param name="types">Фильтр по типам обсуждений из <see cref="ApiDiscussionType"/>. Необязательно.</param>
    /// <param name="category">Категория обсуждений (<see cref="ApiDiscussionTypeFilter"/>). Необязательно.</param>
    /// <param name="fields">Дополнительные поля ответа. Необязательно.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    AnchorNavigator<TDto> GetListNavigator<TDto>(
        IRequestContext context,
        AnchorConfiguration cfg,
        ICollection<ApiDiscussionType>? types = null,
        ApiDiscussionTypeFilter? category = null,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Получает детальную информацию об одном обсуждении (<c>discussions.get</c>).
    /// </summary>
    /// <param name="discussionId">Идентификатор обсуждаемого объекта.</param>
    /// <param name="discussionType">Тип объекта (<see cref="ApiDiscussionType"/>).</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="fields">Дополнительные поля. Необязательно.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Объект дискуссии или <c>null</c>.</returns>
    Task<TDto?> GetAsync<TDto>(
        string discussionId,
        string discussionType,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Получает один комментарий по идентификатору (<c>discussions.getComment</c>).
    /// </summary>
    /// <param name="discussionId">Идентификатор обсуждения.</param>
    /// <param name="discussionType">Тип обсуждения.</param>
    /// <param name="commentId">Идентификатор комментария.</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Детальные данные комментария или <c>null</c>.</returns>
    Task<CommentDetailData?> GetCommentAsync(
        string discussionId,
        string discussionType,
        string commentId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список комментариев к обсуждению (<c>discussions.getDiscussionComments</c>).
    /// Использует offset-пагинацию (не anchor).
    /// </summary>
    /// <param name="discussionId">Идентификатор обсуждения (параметр <c>entityId</c>).</param>
    /// <param name="discussionType">Тип обсуждения (параметр <c>entityType</c>).</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="offset">Смещение для пагинации.</param>
    /// <param name="count">Количество возвращаемых комментариев.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция комментариев или <c>null</c>.</returns>
    Task<ICollection<CommentDetailData>?> GetDiscussionCommentsAsync(
        string discussionId,
        string discussionType,
        IRequestContext context,
        int offset = 0,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество комментариев к обсуждению (<c>discussions.getDiscussionCommentsCount</c>).
    /// </summary>
    /// <param name="discussionId">Идентификатор обсуждения (параметр <c>entityId</c>).</param>
    /// <param name="discussionType">Тип обсуждения (параметр <c>entityType</c>).</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество комментариев.</returns>
    Task<int> GetDiscussionCommentsCountAsync(
        string discussionId,
        string discussionType,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для постраничного перебора лайков обсуждения (<c>discussions.getDiscussionLikes</c>).
    /// </summary>
    /// <param name="discussionId">Идентификатор обсуждения.</param>
    /// <param name="discussionType">Тип обсуждения.</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cfg">Конфигурация курсорной пагинации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    AnchorNavigator<UserLikeDto> GetDiscussionLikesNavigator(
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор для постраничного перебора лайков комментария (<c>discussions.getCommentLikes</c>).
    /// </summary>
    /// <param name="commentId">Идентификатор комментария.</param>
    /// <param name="discussionId">Идентификатор обсуждения.</param>
    /// <param name="discussionType">Тип обсуждения.</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cfg">Конфигурация курсорной пагинации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    AnchorNavigator<UserLikeDto> GetCommentLikesNavigator(
        string commentId,
        string discussionId,
        string discussionType,
        IRequestContext context,
        AnchorConfiguration cfg,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает вложенные ресурсы (фото, видео и т.п.) по идентификаторам вложений
    /// (<c>discussions.getAttachedResources</c>).
    /// </summary>
    /// <param name="attachIds">Список идентификаторов вложений (параметр <c>attach_ids</c>).</param>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция вложенных ресурсов или <c>null</c>.</returns>
    Task<ICollection<AttachedResourceData>?> GetAttachedResourcesAsync(
        ICollection<string> attachIds,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список обсуждений с фильтрацией (<c>discussions.getDiscussions</c>).
    /// </summary>
    /// <remarks>
    /// ⚠️ Метод устарел (deprecated). Рекомендуется использовать <see cref="GetListNavigator{TDto}"/>.
    /// </remarks>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="entityTypes">Фильтр по типам обсуждений (CSV значений <see cref="ApiDiscussionType"/>). Необязательно.</param>
    /// <param name="category">Категория обсуждений. Необязательно.</param>
    /// <param name="offset">Смещение для пагинации.</param>
    /// <param name="count">Количество элементов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция обсуждений или <c>null</c>.</returns>
    [Obsolete("Используйте GetListNavigator<TDto>. Метод deprecated в API OK.ru.")]
    Task<ICollection<DiscussionData>?> GetDiscussionsAsync(
        IRequestContext context,
        ICollection<string>? entityTypes = null,
        string? category = null,
        int offset = 0,
        int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает новости обсуждений для текущего пользователя (<c>discussions.getDiscussionsNews</c>).
    /// </summary>
    /// <param name="context">Контекст запроса: <c>MainAccountRequestContext</c> или <c>ExplicitTokenRequestContext</c>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция новостных элементов или <c>null</c>.</returns>
    Task<ICollection<DiscussionNewsItemData>?> GetDiscussionsNewsAsync(
        IRequestContext context,
        CancellationToken cancellationToken = default);
}