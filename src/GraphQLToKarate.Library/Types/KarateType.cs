namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents a Karate schema type with on information about nullability, non-nullability, or list-ness.
/// </summary>
internal sealed class KarateType(string schema, string name) : KarateTypeBase
{
    public override string Name { get; } = name;

    public override string Schema { get; } = schema;
}
