using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class KarateNullType : KarateTypeBase
{
    private readonly KarateTypeBase _innerType;

    public KarateNullType(KarateTypeBase innerType) => _innerType = innerType;

    public override string Name => _innerType.Name;

    public override string Schema => $"{KarateToken.Null}{_innerType.Schema}";
}
