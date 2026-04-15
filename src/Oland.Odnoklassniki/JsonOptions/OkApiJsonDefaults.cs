using System.Text.Json;
using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.JsonOptions;

public static class OkApiJsonDefaults
{
    public static readonly JsonSerializerOptions Default = new()
    {
        // ОК использует snake_case и иногда camelCase
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        
        // Игнорировать null при сериализации (экономит трафик)
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        
        // Для enum-полей (если будут)
        Converters = { 
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) 
        },
        
        // Опционально: разрешить комментарии (полезно при отладке)
        ReadCommentHandling = JsonCommentHandling.Skip,
    };
}