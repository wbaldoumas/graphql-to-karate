using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

/// <summary>
///     Represents a non-nullable Karate schema type.
/// </summary>
internal sealed class KarateNonNullType(KarateTypeBase innerType) : KarateTypeBase
{
    public override string Name => innerType.Name;

    public override string Schema => $"{KarateToken.NonNull}{innerType.Schema}";
}
