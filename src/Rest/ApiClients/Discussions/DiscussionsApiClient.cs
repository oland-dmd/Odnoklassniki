using Odnoklassniki.Interfaces;
using Odnoklassniki.Interfaces.RestApiClients;
using Odnoklassniki.Rest.ApiClients.Discussions.Datas;
using Odnoklassniki.Rest.ApiClients.Discussions.Enums;
using Odnoklassniki.Rest.ApiClients.Discussions.Responses;
using Odnoklassniki.Rest.Extensions;

namespace Odnoklassniki.Rest.ApiClients.Discussions;

/// <summary>
/// Клиент для взаимодействия с API обсуждений (discussions) социальной сети «Одноклассники».
/// </summary>
public class DiscussionsApiClient(IOkApiClientCore okApi) : IDiscussionsApiClient
{
    private const string OkClassName = "discussions";
    private const string GetListMethodName = $"{OkClassName}.getList";
    private const string GetCommentsMethodName = $"{OkClassName}.getComments";

    // === PUBLIC API (из интерфейса) ===

    public Task<ICollection<DiscussionData>> GetGroupListAsync(
        string accessToken,
        string sessionSecretKey,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        // Делегируем внутреннему методу с фиксированной категорией
        return GetListInternalAsync(accessToken, sessionSecretKey, Category.GROUP, count, cancellationToken);
    }

    public Task<ICollection<DiscussionData>> GetUserListAsync(
        string accessToken,
        string sessionSecretKey,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        return GetListInternalAsync(accessToken, sessionSecretKey, Category.MY, count, cancellationToken);
    }

    public async Task<ICollection<CommentData>> GetCommentsAsync(
        string accessToken,
        string sessionSecretKey,
        string discussionId,
        string discussionType,
        int count = 100,
        CancellationToken cancellationToken = default)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "comment.AUTHOR_ID", "comment.AUTHOR_NAME", "comment.AUTHOR_REF",
                "comment.REPLY_TO_ID", "comment.REPLY_TO_NAME", "comment.REPLY_TO_COMMENT_ID",
                "comment.DATE", "comment.ID", "comment.TEXT", "comment.TYPE")
            .InsertCount(count)
            .InsertCustomParameter("discussionType", discussionType)
            .InsertCustomParameter("discussionId", discussionId)
            .InsertCustomParameter("mark_as_read", true)
            .InsertCustomParameter("includeRemoved", true);

        var response = await okApi.CallAsync<CommentResponse>(
            GetCommentsMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return response.Comments.Select(item => new CommentData
        {
            AuthorId = item.AuthorId,
            Timestamp = item.Timestamp,
            ID = item.ID,
            ReplyToCommentId = item.ReplyToCommentId,
            ReplyToId = item.ReplyToId,
            ReplyToName = item.ReplyToName,
            Text = item.Text,
            Type = item.Type,
            AuthorName = item.AuthorName,
            AuthorRef = item.AuthorRef
        }).ToArray();
    }

    // === INTERNAL IMPLEMENTATION ===

    /// <summary>
    /// Внутренняя реализация получения списка обсуждений.
    /// Не выносится в интерфейс — используется только внутри класса.
    /// </summary>
    private async Task<ICollection<DiscussionData>> GetListInternalAsync(
        string accessToken,
        string sessionSecretKey,
        Category category,
        int count,
        CancellationToken cancellationToken)
    {
        var parameters = new RestParameters()
            .InsertFields(
                "discussion.OBJECT_TYPE", "discussion.OBJECT_ID", "discussion.NEW_COMMENTS_COUNT",
                "group_album.AID", "group.UID", "album.AID", "discussion.TITLE")
            .InsertCount(count)
            .InsertCustomParameter("category", category);

        var response = await okApi.CallAsync<DiscussionResponse>(
            GetListMethodName, accessToken, sessionSecretKey, parameters, cancellationToken: cancellationToken);

        return response.Discussions.Select(item =>
            new DiscussionData
            {
                ID = item.ID,
                NewCommentCount = item.NewCommentCount,
                AlbumId = item.RefObjects.ExtractAlbumId(),
                GroupId = item.RefObjects.ExtractGroupId(),
                Type = item.Type,
                Title = item.Title
            }).ToArray();
    }
}