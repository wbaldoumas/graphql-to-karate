using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents a nullable Karate schema type.
/// </summary>
internal sealed class KarateNullType(KarateTypeBase innerType) : KarateTypeBase
{
    public override string Name => innerType.Name;

    public override string Schema => $"{KarateToken.Null}{innerType.Schema}";
}
