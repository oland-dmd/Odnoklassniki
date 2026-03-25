namespace Odnoklassniki.Attributes;

internal class OkFieldNameAttribute : Attribute
{
    public required string Name { get; init; }
}
