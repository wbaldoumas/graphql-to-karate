using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents a list Karate schema type.
/// </summary>
internal sealed class KarateListType(KarateTypeBase innerType) : KarateTypeBase
{
    public override string Name => innerType.Name;

    public override string Schema => $"{KarateToken.Array} {innerType.Schema}";
}
