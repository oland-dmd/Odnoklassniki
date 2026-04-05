using Microsoft.Extensions.DependencyInjection;
using Oland.Odnoklassniki.Image;
using Oland.Odnoklassniki.Interfaces;
using Oland.Odnoklassniki.Interfaces.RestApiClients;
using Oland.Odnoklassniki.Rest.ApiClientCore;
using Oland.Odnoklassniki.Rest.ApiClients.Auth;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions;
using Oland.Odnoklassniki.Rest.ApiClients.Friends;
using Oland.Odnoklassniki.Rest.ApiClients.Groups;
using Oland.Odnoklassniki.Rest.ApiClients.Photos;
using Oland.Odnoklassniki.Rest.ApiClients.PhotosV2;
using Oland.Odnoklassniki.Rest.ApiClients.Users;

namespace Oland.Odnoklassniki.Extensions;

/// <summary>
/// Класс расширений для регистрации клиентов API Одноклассников в контейнере зависимостей (DI).
/// Предоставляет централизованный механизм настройки всех компонентов для работы с OK.ru API.
/// </summary>
/// <remarks>
/// Рекомендуется вызывать метод <see cref="AddOkApiClients"/> при инициализации приложения
/// (например, в <c>Program.cs</c> или <c>Startup.cs</c>) для автоматической регистрации всех необходимых сервисов.
/// </remarks>
public static class ServiceExtensions
{
    /// <summary>
    /// Регистрирует все клиенты API Одноклассников в контейнере зависимостей с временем жизни <c>Scoped</c>.
    /// </summary>
    /// <remarks>
    /// Метод выполняет следующую конфигурацию:
    /// <list type="bullet">
    /// <item>Регистрирует базовый клиент <see cref="IOkApiClientCore"/> для выполнения подписанных запросов;</item>
    /// <item>Настраивает привязку опций <see cref="ApplicationOptions"/> к секции конфигурации <c>"OkApi"</c>;</item>
    /// <item>Регистрирует специализированные клиенты для работы с различными разделами API (фото, пользователи, группы и т.д.).</item>
    /// </list>
    /// 
    /// <para><b>Требуемая конфигурация (appsettings.json):</b></para>
    /// <code>
    /// {
    ///   "OkApi": {
    ///     "ApplicationKey": "your_app_key",
    ///     "AccessToken": "your_access_token",
    ///     "SessionSecretKey": "your_session_secret"
    ///   }
    /// }
    /// </code>
    /// 
    /// <para><b>Зарегистрированные сервисы:</b></para>
    /// <list type="bullet">
    /// <item><see cref="IOkApiClientCore"/> — базовый клиент для вызова методов API;</item>
    /// <item><see cref="IAlbumsApiClient"/> — управление альбомами;</item>
    /// <item><see cref="IPhotosApiClient"/> — работа с фотографиями (legacy);</item>
    /// <item><see cref="IPhotosV2ApiClient"/> — работа с фотографиями (версия 2);</item>
    /// <item><see cref="IAuthApiClient"/> — авторизация и сессии;</item>
    /// <item><see cref="IDiscussionsApiClient"/> — обсуждения и комментарии;</item>
    /// <item><see cref="IUserApiClient"/> — информация о пользователях;</item>
    /// <item><see cref="IFriendsApiClient"/> — управление списком друзей;</item>
    /// <item><see cref="IGroupsApiClient"/> — управление группами;</item>
    /// <item><see cref="ImageClient"/> — клиент для загрузки и обработки изображений.</item>
    /// </list>
    /// </remarks>
    /// <param name="services">Коллекция сервисов для регистрации зависимостей.</param>
    /// <returns>Та же коллекция сервисов для поддержки цепочки вызовов (fluent interface).</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если параметр <paramref name="services"/> равен <see langword="null"/>.
    /// </exception>
    /// <exception cref="Microsoft.Extensions.Options.OptionsValidationException">
    /// Возникает при отсутствии или некорректном заполнении секции конфигурации <c>"OkApi"</c>.
    /// </exception>
    public static IServiceCollection AddOkApiClients(this IServiceCollection services)
    {
        services.AddScoped<IOkApiClientCore, OkApiClientCore>();
        services.AddOptions<ApplicationOptions>()
            .BindConfiguration("OkApi");

        services.AddScoped<IAlbumsApiClient, AlbumsApiClient>()
            .AddScoped<IPhotosApiClient, PhotosApiClient>()
            .AddScoped<IPhotosV2ApiClient, PhotosV2ApiClient>()
            .AddScoped<IAuthApiClient, AuthApiClient>()
            .AddScoped<IDiscussionsApiClient, DiscussionsApiClient>()
            .AddScoped<IUserApiClient, UserApiClient>()
            .AddScoped<IFriendsApiClient, FriendsApiClient>()
            .AddScoped<IGroupsApiClient, GroupsApiClient>()
            .AddScoped<ImageClient>();

        return services;
    }
}