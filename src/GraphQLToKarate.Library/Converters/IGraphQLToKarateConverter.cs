namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts GraphQL schemas to Karate features.
/// </summary>
public interface IGraphQLToKarateConverter
{
    /// <summary>
    ///     Converts the given GraphQL <paramref name="schema"/> to a Karate feature.
    /// </summary>
    /// <param name="schema">The source GraphQL schema to convert.</param>
    /// <returns>The converted Karate feature.</returns>
    string Convert(string schema);
}