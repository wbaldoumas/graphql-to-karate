namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents a Karate schema type with on information about nullability, non-nullability, or list-ness.
/// </summary>
internal sealed class KarateType : KarateTypeBase
{
    public KarateType(string schema, string name)
    {
        Schema = schema;
        Name = name;
    }

    public override string Name { get; }

    public override string Schema { get; }
}
