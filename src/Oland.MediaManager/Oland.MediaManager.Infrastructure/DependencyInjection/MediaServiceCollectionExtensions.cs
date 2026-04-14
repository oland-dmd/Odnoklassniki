using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Application.Options;
using Oland.MediaManager.Application.Services;
using Oland.MediaManager.Application.Validation;

namespace Oland.MediaManager.Application.DependencyInjection;

    /// <summary>
    /// Методы расширения для регистрации медиа-сервиса в DI-контейнере
    /// </summary>
    public static class MediaServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует базовые сервисы медиа-менеджера с настройками по умолчанию
        /// </summary>
        public static IServiceCollection AddMediaManager(
            this IServiceCollection services)
        {
            return AddMediaManager(services, _ => { });
        }

        /// <summary>
        /// Регистрирует сервисы с кастомной конфигурацией
        /// </summary>
        public static IServiceCollection AddMediaManager(
            this IServiceCollection services,
            Action<MediaServiceOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureOptions);

            // Регистрируем опции
            services.Configure(configureOptions);

            // Регистрируем зависимости
            services.AddSingleton<IMediaValidator, MediaValidator>();
            
            // Регистрируем основной сервис с фабрикой для доступа к опциям
            services.AddSingleton<IMediaService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MediaServiceOptions>>().Value;
                var validator = sp.GetService<IMediaValidator>();
                var jsonOptions = options.ToJsonOptions();
                
                return new MediaService(validator, jsonOptions);
            });

            // Регистрируем builder как transient (для использования вне сервиса)
            services.AddTransient<MediaCollectionBuilder>();

            return services;
        }

        /// <summary>
        /// Регистрирует сервисы, читая настройки из конфигурации (appsettings.json)
        /// </summary>
        /// <remarks>
        /// Ожидает секцию "MediaService" в конфигурации:
        /// {
        ///   "MediaService": {
        ///     "AutoValidate": true,
        ///     "WriteIndented": false
        ///   }
        /// }
        /// </remarks>
        public static IServiceCollection AddMediaManager(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<MediaServiceOptions>? configureOverrides = null)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            return AddMediaManager(services, options =>
            {
                configuration.GetSection("MediaService").Bind(options);
                configureOverrides?.Invoke(options);
            });
        }
    }
