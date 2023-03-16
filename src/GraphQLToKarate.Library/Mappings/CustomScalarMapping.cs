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

    /// <summary>
    ///     Returns the custom scalar mapping as a string, formatted in a way that is acceptable as a command-line argument.
    ///
    ///     An example response would be "DateTime:string,URL:string,Long:int".
    /// </summary>
    /// <returns>The custom scalar mapping as a string.</returns>
    public override string ToString() => string.Join(',', Mappings.Select(mapping => $"{mapping.Key}:{mapping.Value}"));
}