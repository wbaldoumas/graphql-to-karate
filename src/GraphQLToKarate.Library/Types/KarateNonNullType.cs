using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class KarateNonNullType : KarateTypeBase
{
    private readonly KarateTypeBase _innerType;

    public KarateNonNullType(KarateTypeBase innerType) => _innerType = innerType;

    public override string Name => _innerType.Name;

    public override string Schema => $"{KarateToken.NonNull}{_innerType.Schema}";
}
