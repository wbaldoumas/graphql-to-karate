﻿using GraphQLToKarate.Library.Tokens;

namespace GraphQLToKarate.Library.Types;

internal sealed class KarateListType : KarateTypeBase
{
    private readonly KarateTypeBase _innerType;

    public KarateListType(KarateTypeBase innerType) => _innerType = innerType;

    public override string Name => _innerType.Name;

    public override string Schema => $"{KarateToken.Array} {_innerType.Schema}";
}
