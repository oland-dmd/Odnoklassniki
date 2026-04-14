using System.Text.Json;
using System.Text.Json.Serialization;
using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Application.Exceptions;
using Oland.MediaManager.Application.Validation;
using Oland.MediaManager.Domain.MediaItems;

namespace Oland.MediaManager.Application.Services;

    public class MediaService : IMediaService
    {
        private readonly IMediaValidator? _validator;
        private readonly JsonSerializerOptions _jsonOptions;

        public MediaService(
            IMediaValidator? validator = null, 
            JsonSerializerOptions? jsonOptions = null)
        {
            _validator = validator;
            _jsonOptions = jsonOptions ?? MediaCollection.DefaultOptions;
        }

        public MediaCollection Create(Action<MediaCollectionBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            
            var builder = new MediaCollectionBuilder();
            configure(builder);
            return builder.Build();
        }

        public string Serialize(MediaCollection collection, bool validate = true)
        {
            ArgumentNullException.ThrowIfNull(collection);
            
            if (validate)
            {
                var result = Validate(collection);
                if (!result.IsValid)
                    throw new MediaValidationException(result.Errors);
            }

            return JsonSerializer.Serialize(
                new { media = collection.Items }, 
                _jsonOptions);
        }

        public async Task<string> SerializeAsync(
            MediaCollection collection, 
            bool validate = true, 
            CancellationToken ct = default)
        {
            // Сериализация в .NET 8+ поддерживает async, но для совместимости:
            await Task.Yield(); 
            return Serialize(collection, validate);
        }

        public ValidationResult Validate(MediaCollection collection)
        {
            ArgumentNullException.ThrowIfNull(collection);
            
            return _validator is null ? ValidationResult.Ok : _validator.Validate(collection);
        }

        public MediaCollection? Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var wrapper = JsonSerializer.Deserialize<MediaWrapper>(json, _jsonOptions);
            return wrapper?.Media is { Count: > 0 } items 
                ? new MediaCollection(items) 
                : null;
        }

        public string CreateAndSerialize(
            Action<MediaCollectionBuilder> configure, 
            bool validate = true)
        {
            var collection = Create(configure);
            return Serialize(collection, validate);
        }

        // Вспомогательная запись для десериализации
        private record MediaWrapper([property: JsonPropertyName("media")] List<MediaItem> Media);
    }
