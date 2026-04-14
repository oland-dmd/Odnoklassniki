using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common.DynamicIdAliases;

namespace Oland.Odnoklassniki.Common;

/// <summary>
/// Базовый класс для сущностей с динамическим названием поля ID
/// </summary>
public abstract class DynamicIdEntity
{
    private string? _resolvedId;
        
    /// <summary>
    /// Каноничное поле ID (используется в коде)
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id 
    {
        get => _resolvedId ??= ResolveAliases();
        set => _resolvedId = value;
    }

    /// <summary>
    /// Ловушка для всех неизвестных полей JSON
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> UnknownProperties { get; set; } = new();

    /// <summary>
    /// Автоматически вызывается после десериализации
    /// </summary>
    [OnDeserialized]
    internal string? ResolveAliases()
    {
        foreach (var alias in DynamicIdAliasRegistry.KnownAliases)
        {
            if (!UnknownProperties.TryGetValue(alias, out var element)) continue;
            var result = element.ValueKind == JsonValueKind.String 
                ? element.GetString() 
                : element.ToString();
            return result; // Нашли первый совпавший алиас → выходим
        }
        
        return null;
    }
}
