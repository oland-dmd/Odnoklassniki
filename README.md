# Oland.Odnoklassniki

.NET-клиент для работы с REST API социальной сети [Одноклассники (OK.ru)](https://apiok.ru/).

## Поддерживаемые фреймворки

- `net9.0`
- `net10.0`

## Установка

```bash
dotnet add package Oland.Odnoklassniki
```

## Регистрация в DI

```csharp
builder.Services.AddOkApiClients();
```

Для работы необходима конфигурация в `appsettings.json`:

```json
{
  "OkApi": {
    "ApplicationKey": "your_app_key",
    "AccessToken": "your_access_token",
    "SessionSecretKey": "your_session_secret",
    "GroupId": "your_group_id"
  }
}
```

## Реализованные клиенты

| Интерфейс | Описание | Методы |
|---|---|---|
| `IAlbumsApiClient` | Фотоальбомы | `GetAlbumsAsync`, `CreateAlbumAsync`, `EditAlbumAsync`, `SetAlbumMainPhotoAsync` |
| `IPhotosApiClient` | Фотографии (legacy) | `GetPhotoInfoAsync`, `GetPhotosAsync`, `EditPhotoAsync`, `DeletePhotoAsync` |
| `IPhotosV2ApiClient` | Фотографии v2 + расширение | `GetPhotosInfoAsync`, `GetUserPhotosNavigator`, `GetUserAlbumPhotosNavigator`, `AddPhotoLikeAsync`, `AddAlbumLikeAsync`, `GetPhotoLikesNavigator`, `GetAlbumLikesNavigator`, `GetPhotoMarksAsync`, `GetTagsAsync`, `DeleteTagsAsync`, `GetUploadUrlAsync`, `UploadAsync` |
| `IGroupsApiClient` | Группы | `GetGroupsInfoAsync`, `GetUserGroupsInfoByIdsAsync`, `GetUserGroupsAnchorNavigator`, `GetMembersNavigator`, `GetCountersAsync`, `GetStatOverviewAsync`, `GetStatPeopleAsync`, `GetStatTopicAsync`, `GetStatTopicsNavigator`, `GetStatTrendsAsync`, `PinGroupFeedAsync`, `SetMainPhotoAsync`, `IsMessagesAllowedAsync` |
| `IVideoApiClient` | Видео | `GetUploadUrlAsync`, `UpdateAsync`, `DeleteAsync`, `SubscribeAsync` |
| `IShareApiClient` | Внешние ссылки | `FetchLinkAsync` |
| `IMediaTopicsApiClient` | Посты / медиатопики | `PostAsync`, `EditAsync`, `DeleteTopicAsync`, `GetByIdsAsync`, `GetRepublishedTopicAsync`, `GetPollVotersNavigator` |
| `IStreamApiClient` | Лента | `DeleteAsync`, `MarkAsSpamAsync`, `IsSubscribedAsync` |
| `IDiscussionsApiClient` | Обсуждения | `GetGroupListAsync`, `GetUserListAsync`, `GetCommentsAsync`, `GetListNavigator`, `GetAsync`, `GetCommentAsync`, `GetDiscussionCommentsAsync`, `GetDiscussionCommentsCountAsync`, `GetDiscussionLikesNavigator`, `GetCommentLikesNavigator`, `GetAttachedResourcesAsync`, `GetDiscussionsNewsAsync` |
| `IUserApiClient` | Пользователи | `GetLoggedInUserAsync`, `GetCurrentUserAsync`, `GetInfoAsync`, `GetInfoByAsync`, `GetAdditionalInfoAsync`, `SetStatusAsync`, `HasAppPermissionAsync`, `IsAppUserAsync`, `GetCallsLeftAsync` |
| `IFriendsApiClient` | Друзья | `GetUserFriendsAsync`, `GetAppUsersAsync`, `GetOnlineAsync`, `GetMutualFriendsAsync`, `GetByDevicesAsync`, `GetBirthdaysAsync`, `GetSuggestionsAsync` |
| `IAuthApiClient` | Авторизация | методы работы с сессиями |
| `ImageClient` | Загрузка изображений | `UploadAsync` (multipart) |

> **`IPhotosApiClient.EditPhotoAsync` возвращает `bool`** — это ответ метода OK `photos.editPhoto`
> (права `PHOTO_CONTENT` + `VALUABLE_ACCESS`): `false` означает, что OK принял вызов, но правку
> **не применил** (текст фото не изменился), при этом ошибки нет. Для групповых фото (`gid`) правка
> применяется; для фото **страниц** OK систематически возвращает `false` — вызывающая сторона обязана
> проверять результат и иметь запасной путь (перезаливку фото). Пустая `description` отклоняется
> клиентом (`ArgumentException`): отсутствующий параметр стирает описание фото.

## Контексты запросов

| Контекст | Описание |
|---|---|
| `MainAccountRequestContext` | Основной аккаунт (из конфигурации) |
| `MainGroupRequestContext` | Основной аккаунт + группа (из конфигурации) |
| `GroupRequestContext` | Произвольная группа с опциональным токеном |
| `ExplicitTokenRequestContext` | Произвольный пользовательский токен |
| `FriendRequestContext` | Запрос в контексте друга |

## Пагинация

Курсорная пагинация реализована через `AnchorNavigator<T>`:

```csharp
var navigator = groupsClient.GetMembersNavigator(context, new AnchorConfiguration { Count = 50 });
while (navigator.HasMore)
{
    var page = await navigator.NextPageAsync();
    // page.Results — список GroupMemberDto
}
```
