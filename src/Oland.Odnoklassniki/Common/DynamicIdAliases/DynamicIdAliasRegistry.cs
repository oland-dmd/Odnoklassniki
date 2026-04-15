namespace Oland.Odnoklassniki.Common.DynamicIdAliases;

/// <summary>
/// Глобальный реестр возможных названий для поля ID
/// </summary>
public static class DynamicIdAliasRegistry
{
    // HashSet для O(1) поиска + порядок важен (приоритет маппинга)
    private static readonly LinkedList<string> _aliases = new();
        
    public static IEnumerable<string> KnownAliases => _aliases;

    static DynamicIdAliasRegistry()
    {
        // Базовый набор, который покрывает 95% кейсов
        Register("catalog_id", "product_id");
    }

    /// <summary>
    /// Добавить новые алиасы (можно вызывать при старте приложения)
    /// </summary>
    public static void Register(params string[] aliases)
    {
        foreach (var alias in aliases)
            _aliases.AddLast(alias);
    }

    /// <summary>
    /// Очистить и задать свой набор (для тестов или изоляции)
    /// </summary>
    public static void Override(IEnumerable<string> aliases)
    {
        _aliases.Clear();
        foreach (var a in aliases) _aliases.AddLast(a);
    }
}
