namespace Oland.Odnoklassniki.Rest.RequestContexts.ValueObjects;

public class CatalogId
{
    public string Value { get; }

    public CatalogId(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}