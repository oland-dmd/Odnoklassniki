using Oland.Odnoklassniki.Rest.RequestContexts;

namespace Oland.Odnoklassniki.Interfaces.RestApiClients;

/// <summary>
/// Клиент REST API Одноклассников для управления лентой пользователя.
/// Охватывает методы класса <c>stream.*</c>.
/// </summary>
/// <remarks>
/// <para>
/// Все методы требуют scope <c>VALUABLE_ACCESS</c> и поддерживают только пользовательские контексты
/// (<see cref="MainAccountRequestContext"/>, <see cref="ExplicitTokenRequestContext"/>).
/// </para>
/// <para>
/// <b>Важно:</b> параметры <c>deleteId</c> и <c>markAsSpamId</c> — это одноразовые токены,
/// а не обычные идентификаторы топиков. Их нужно получить предварительно через
/// <c>mediatopic.getByIds</c> с полями <c>delete_id</c> / <c>mark_as_spam_id</c>.
/// </para>
/// </remarks>
public interface IStreamApiClient
{
    /// <summary>
    /// Удаляет запись из ленты пользователя через метод <c>stream.delete</c>.
    /// </summary>
    /// <param name="deleteId">
    /// Одноразовый токен удаления. Получается из поля <c>delete_id</c> медиатопика
    /// при вызове <c>mediatopic.getByIds</c> с соответствующим полем в <c>fields</c>.
    /// </param>
    /// <param name="context">
    /// Контекст запроса. Допустимы только <see cref="MainAccountRequestContext"/>
    /// и <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/> при успешном удалении.</returns>
    Task<bool> DeleteAsync(
        string deleteId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Помечает запись в ленте как спам через метод <c>stream.markAsSpam</c>.
    /// </summary>
    /// <param name="markAsSpamId">
    /// Одноразовый токен пометки как спам. Получается из поля <c>mark_as_spam_id</c> медиатопика
    /// при вызове <c>mediatopic.getByIds</c> с соответствующим полем в <c>fields</c>.
    /// </param>
    /// <param name="context">
    /// Контекст запроса. Допустимы только <see cref="MainAccountRequestContext"/>
    /// и <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/> при успешной пометке.</returns>
    Task<bool> MarkAsSpamAsync(
        string markAsSpamId,
        IRequestContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, подписан ли текущий пользователь на ленту владельца через метод <c>stream.isSubscribed</c>.
    /// </summary>
    /// <param name="ownerId">Идентификатор пользователя (<c>fid</c>) или группы (<c>gid</c>), чья лента проверяется.</param>
    /// <param name="isGroup">
    /// <see langword="true"/> — <paramref name="ownerId"/> является группой (передаётся как <c>gid</c>);
    /// <see langword="false"/> — пользователь (передаётся как <c>fid</c>).
    /// Параметры взаимоисключающие по документации API.
    /// </param>
    /// <param name="context">
    /// Контекст запроса. Допустимы только <see cref="MainAccountRequestContext"/>
    /// и <see cref="ExplicitTokenRequestContext"/>.
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns><see langword="true"/> если пользователь подписан на указанную ленту.</returns>
    Task<bool> IsSubscribedAsync(
        string ownerId,
        bool isGroup,
        IRequestContext context,
        CancellationToken cancellationToken = default);
}
