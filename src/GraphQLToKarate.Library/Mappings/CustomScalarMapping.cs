namespace GraphQLToKarate.Library.Mappings;

/// <inheritdoc cref="ICustomScalarMapping"/>
public sealed class CustomScalarMapping : ICustomScalarMapping
{
    public IDictionary<string, string> Mappings { get; }

    public CustomScalarMapping() => Mappings = new Dictionary<string, string>();

    public CustomScalarMapping(IDictionary<string, string> mappings) =>
        Mappings = mappings;

    public bool TryGetKarateType(string graphQLType, out string karateType) =>
        Mappings.TryGetValue(graphQLType, out karateType!);

    public bool Any() => Mappings.Any();
}