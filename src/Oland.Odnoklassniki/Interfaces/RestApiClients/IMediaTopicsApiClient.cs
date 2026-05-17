using Oland.MediaManager.Application.Builders;
using Oland.Odnoklassniki.Common;
using Oland.Odnoklassniki.Rest.AnchorNavigators;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Datas;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Models;
using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент REST API Одноклассников для работы с медиатопиками (посты, статусы, темы групп).
/// Охватывает методы класса <c>mediatopic.*</c>.
/// </summary>
/// <remarks>
/// Требует scope <c>VALUABLE_ACCESS</c>; для постинга в группу — <c>GROUP_CONTENT</c>;
/// для установки статуса — <c>SET_STATUS</c>.
/// </remarks>
public interface IMediaTopicsApiClient
{
    /// <summary>
    /// Публикует новый медиатопик через метод <c>mediatopic.post</c>.
    /// </summary>
    /// <param name="buildAttachment">
    /// Делегат для конфигурации медиа-вложения (текст, фото, ссылки, опросы) через <see cref="MediaCollectionBuilder"/>.
    /// </param>
    /// <param name="options">Параметры публикации: тип, дата, видимость, настройки комментариев и т.п.</param>
    /// <param name="context">
    /// Контекст запроса. Допустимы: <see cref="GroupRequestContext"/>, <see cref="MainGroupRequestContext"/>,
    /// <see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданного топика или <see langword="null"/> при ошибке.</returns>
    Task<string?> PostAsync(
        Action<MediaCollectionBuilder> buildAttachment,
        MediaTopicPostOptions options,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирует существующий медиатопик через метод <c>mediatopic.edit</c>.
    /// </summary>
    /// <param name="topicId">Идентификатор топика.</param>
    /// <param name="buildAttachment">Делегат конфигурации нового медиа-вложения.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/> при успешном обновлении.</returns>
    Task<bool> EditAsync(
        string topicId,
        Action<MediaCollectionBuilder> buildAttachment,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет медиатопик через метод <c>mediatopic.deleteTopic</c>.
    /// </summary>
    /// <param name="topicId">Идентификатор топика.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/> при успешном удалении.</returns>
    Task<bool> DeleteTopicAsync(
        string topicId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает медиатопики по идентификаторам через метод <c>mediatopic.getByIds</c>.
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, наследующий <see cref="BaseOkDto"/>.</typeparam>
    /// <param name="topicIds">Коллекция идентификаторов топиков.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="fields">Список полей для проекции ответа. По умолчанию — <c>id</c>.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция топиков или пустой список.</returns>
    Task<ICollection<TDto>> GetByIdsAsync<TDto>(
        ICollection<string> topicIds,
        IRequestContext context,
        IEnumerable<string>? fields = null,
        CancellationToken cancellationToken = default) where TDto : BaseOkDto;

    /// <summary>
    /// Возвращает идентификатор репоста для топика через метод <c>mediatopic.getRepublishedTopic</c>.
    /// </summary>
    /// <param name="topicId">Идентификатор оригинального топика.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор репоста или <see langword="null"/>.</returns>
    Task<string?> GetRepublishedTopicAsync(
        string topicId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает навигатор по пользователям, проголосовавшим за вариант ответа в опросе.
    /// Использует метод <c>mediatopic.getPollAnswerVoters</c>.
    /// </summary>
    /// <param name="topicId">Идентификатор топика с опросом.</param>
    /// <param name="answerIndex">Индекс варианта ответа (начиная с 0).</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="anchorConfiguration">Конфигурация курсорной навигации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Навигатор по голосующим пользователям.</returns>
    AnchorNavigator<PollVoterDto> GetPollVotersNavigator(
        string topicId,
        int answerIndex,
        IRequestContext context,
        AnchorConfiguration anchorConfiguration,
        CancellationToken cancellationToken = default);
}
