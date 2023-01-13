namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents an abstract Karate schema type.
/// </summary>
public abstract class KarateTypeBase
{
    public abstract string Name { get; }

    public abstract string Schema { get; }
}
